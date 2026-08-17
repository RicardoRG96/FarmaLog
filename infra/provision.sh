#!/usr/bin/env bash
#
# FarmaLog — aprovisionamiento de la rebanada 1 (walking skeleton).
#
# IDEMPOTENTE: converge al estado deseado. Se puede correr todos los días,
# con el entorno vacío, a medias o completo. Descubre lo que ya existe en el
# resource group y solo crea lo que falta.
#
# Requiere sesión previa:  az login --use-device-code
# Uso:                     bash infra/provision.sh
# Apagar lo que cuesta:    bash infra/teardown.sh
#
# NO autentica y NO contiene secretos: la autenticación es contexto de
# ejecución, no parte del contrato de infraestructura.

set -euo pipefail

# ---------------------------------------------------------------------------
# Parámetros
# ---------------------------------------------------------------------------
LOCATION="westus3"                 # el free offer de Azure SQL NO está en chilecentral
RG="rg-farmalog"

QUEUE="registrar-solicitud-ingreso"

SQL_DB="farmalog-nucleo"
SQL_ADMIN="farmalogadmin"

WORKER_DIR="src/FarmaLog.Nucleo.Worker"
ENV_FILE=".env.local"
SECRETS_FILE=".secrets-comandos.local"

# ---------------------------------------------------------------------------
# Chequeos previos — el script VERIFICA la sesión, no la crea.
# ---------------------------------------------------------------------------
az account show -o none 2>/dev/null \
  || { echo "No hay sesión de Azure. Corré 'az login --use-device-code' primero."; exit 1; }

# El script escribe secretos en la raíz del repo: se niega a correr si esos
# archivos pueden terminar en un commit.
for ARCHIVO in "$ENV_FILE" "$SECRETS_FILE"; do
  grep -qxF "$ARCHIVO" .gitignore 2>/dev/null \
    || { echo "FALTA en .gitignore: $ARCHIVO — agregalo antes de continuar."; exit 1; }
done

echo "==> Suscripción activa:"
az account show --query "{nombre:name, id:id}" -o table

read -rp "¿Es la suscripción correcta? [s/N] " CONFIRMA
[[ "$CONFIRMA" == "s" || "$CONFIRMA" == "S" ]] || { echo "Abortado."; exit 1; }

# ---------------------------------------------------------------------------
# Resource providers — no están habilitados por defecto en una suscripción
# nueva. Se consulta antes de registrar: 'register --wait' sobre un provider
# ya registrado cuesta minutos y no aporta nada.
# ---------------------------------------------------------------------------
echo "==> Verificando resource providers"
for RP in Microsoft.ServiceBus Microsoft.Sql Microsoft.App Microsoft.OperationalInsights; do
  ESTADO=$(az provider show --namespace "$RP" --query registrationState -o tsv 2>/dev/null || echo "NotRegistered")
  if [[ "$ESTADO" == "Registered" ]]; then
    echo "    - $RP: ya registrado"
  else
    echo "    - $RP: registrando (puede tardar)"
    az provider register --namespace "$RP" --wait
  fi
done

# ---------------------------------------------------------------------------
# Resource group. Su location es solo metadata.
# ---------------------------------------------------------------------------
if az group show --name "$RG" -o none 2>/dev/null; then
  echo "==> Resource group $RG: ya existe"
else
  echo "==> Creando resource group $RG en $LOCATION"
  az group create --name "$RG" --location "$LOCATION" -o none
fi

# ---------------------------------------------------------------------------
# Service Bus — se descubre antes de crear. El nombre es DNS global, así que
# lleva sufijo aleatorio; pero solo se inventa uno si no hay namespace previo.
# ---------------------------------------------------------------------------
SB_NS=$(az servicebus namespace list -g "$RG" --query "[0].name" -o tsv 2>/dev/null || true)

if [[ -n "$SB_NS" ]]; then
  echo "==> Service Bus: reutilizando namespace existente $SB_NS"
else
  SB_NS="sb-farmalog-$(openssl rand -hex 3)"
  echo "==> Creando namespace $SB_NS (Standard) en $LOCATION — tarda 2 a 4 minutos"
  az servicebus namespace create \
    --resource-group "$RG" \
    --name "$SB_NS" \
    --location "$LOCATION" \
    --sku Standard \
    -o none
fi

if az servicebus queue show -g "$RG" --namespace-name "$SB_NS" -n "$QUEUE" -o none 2>/dev/null; then
  echo "==> Cola $QUEUE: ya existe (no se toca: la detección de duplicados es inmutable)"
else
  echo "==> Creando cola $QUEUE"
  az servicebus queue create \
    --resource-group "$RG" \
    --namespace-name "$SB_NS" \
    --name "$QUEUE" \
    --max-delivery-count 3 \
    --lock-duration PT1M \
    --enable-duplicate-detection true \
    --duplicate-detection-history-time-window PT10M \
    -o none
fi

# ---------------------------------------------------------------------------
# Azure SQL — free offer, serverless, auto-pause.
# La password no se puede leer de Azure: se pide siempre, porque hace falta
# para armar la connection string aunque el servidor ya exista.
# ---------------------------------------------------------------------------
SQL_SERVER=$(az sql server list -g "$RG" --query "[0].name" -o tsv 2>/dev/null || true)

if [[ -n "$SQL_SERVER" ]]; then
  echo "==> Azure SQL: reutilizando servidor existente $SQL_SERVER"
  read -rsp "Password del admin de SQL (la MISMA con la que se creó): " SQL_PASSWORD; echo
else
  SQL_SERVER="sql-farmalog-$(openssl rand -hex 3)"
  read -rsp "Password nueva del admin de SQL (min 8, may+min+numero+simbolo): " SQL_PASSWORD; echo
  echo "==> Creando servidor lógico $SQL_SERVER"
  az sql server create \
    --resource-group "$RG" \
    --name "$SQL_SERVER" \
    --location "$LOCATION" \
    --admin-user "$SQL_ADMIN" \
    --admin-password "$SQL_PASSWORD" \
    -o none
fi

if az sql db show -g "$RG" --server "$SQL_SERVER" -n "$SQL_DB" -o none 2>/dev/null; then
  echo "==> Base $SQL_DB: ya existe"
  BASE_RECIEN_CREADA=false
else
  echo "==> Creando base $SQL_DB (free offer, AutoPause)"
  echo "    Si falla acá con ProvisioningDisabled, el slot del free offer todavía"
  echo "    no se liberó (puede tardar hasta una hora tras borrar la anterior)."
  az sql db create \
    --resource-group "$RG" \
    --server "$SQL_SERVER" \
    --name "$SQL_DB" \
    --edition GeneralPurpose \
    --family Gen5 \
    --capacity 2 \
    --compute-model Serverless \
    --use-free-limit \
    --free-limit-exhaustion-behavior AutoPause \
    --auto-pause-delay 60 \
    -o none
  BASE_RECIEN_CREADA=true
fi

# ---------------------------------------------------------------------------
# Firewall. La regla de la máquina se ACTUALIZA: la IP pública cambia sola.
# ---------------------------------------------------------------------------
echo "==> Regla: permitir servicios de Azure (0.0.0.0 es el flag mágico, no 'internet')"
az sql server firewall-rule create \
  --resource-group "$RG" --server "$SQL_SERVER" \
  --name permitir-servicios-azure \
  --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0 \
  -o none 2>/dev/null || true

MI_IP=$(curl -s https://api.ipify.org)
echo "==> Regla: mi máquina ($MI_IP)"
if az sql server firewall-rule show -g "$RG" --server "$SQL_SERVER" -n mi-maquina -o none 2>/dev/null; then
  az sql server firewall-rule update \
    --resource-group "$RG" --server "$SQL_SERVER" \
    --name mi-maquina \
    --start-ip-address "$MI_IP" --end-ip-address "$MI_IP" \
    -o none
else
  az sql server firewall-rule create \
    --resource-group "$RG" --server "$SQL_SERVER" \
    --name mi-maquina \
    --start-ip-address "$MI_IP" --end-ip-address "$MI_IP" \
    -o none
fi

# ---------------------------------------------------------------------------
# Salida — el script deja el entorno USABLE, no solo creado.
# ---------------------------------------------------------------------------
SB_CONN=$(az servicebus namespace authorization-rule keys list \
  --resource-group "$RG" --namespace-name "$SB_NS" \
  --name RootManageSharedAccessKey --query primaryConnectionString -o tsv)

SQL_CONN="Server=tcp:${SQL_SERVER}.database.windows.net,1433;Initial Catalog=${SQL_DB};User ID=${SQL_ADMIN};Password=${SQL_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;"

# .env.local: alimenta 'docker run --env-file'. El doble guion bajo es el
# separador de jerarquía del proveedor de variables de entorno de .NET.
cat > "$ENV_FILE" <<EOF
ConnectionStrings__ServiceBus=${SB_CONN}
ConnectionStrings__NucleoDb=${SQL_CONN}
EOF
chmod 600 "$ENV_FILE"

# Los comandos van a un archivo, NO a stdout: la pantalla se copia a chats,
# capturas y tickets, y las connection strings llevan la password adentro.
cat > "$SECRETS_FILE" <<EOF
dotnet user-secrets set "ConnectionStrings:ServiceBus" "$SB_CONN" --project $WORKER_DIR
dotnet user-secrets set "ConnectionStrings:NucleoDb" "$SQL_CONN" --project $WORKER_DIR
EOF
chmod 600 "$SECRETS_FILE"

echo
echo "================= NOMBRES ======================================"
echo "SB_NS=$SB_NS"
echo "SQL_SERVER=$SQL_SERVER"
echo
echo "================= SECRETOS (fuera de pantalla) ================="
echo "$ENV_FILE     -> alimenta 'docker run --env-file $ENV_FILE'"

# Los user-secrets son la OTRA fuente de configuración: alimentan al worker en
# Development (IDE y 'dotnet ef'). Se cargan acá para que no puedan divergir
# de .env.local. Va al final y no aborta: si falla, los recursos de Azure ya
# están creados y el script debe terminar informando, no reventar.
# El redirect a /dev/null no es cosmético: 'user-secrets set' imprime el valor.
if command -v dotnet >/dev/null 2>&1 && [[ -d "$WORKER_DIR" ]]; then
  if dotnet user-secrets set "ConnectionStrings:ServiceBus" "$SB_CONN" --project "$WORKER_DIR" >/dev/null 2>&1 \
  && dotnet user-secrets set "ConnectionStrings:NucleoDb"   "$SQL_CONN" --project "$WORKER_DIR" >/dev/null 2>&1; then
    echo "user-secrets   -> cargados en $WORKER_DIR (estado fuera del repo)"
  else
    echo "user-secrets   -> FALLÓ. Cargalos a mano con:  bash $SECRETS_FILE"
  fi
else
  echo "user-secrets   -> omitidos (falta el SDK de .NET o el proyecto)."
  echo "                  Cargalos con:  bash $SECRETS_FILE"
fi

if [[ "$BASE_RECIEN_CREADA" == "true" ]]; then
  echo
  echo "================= LA BASE NACIÓ VACÍA =========================="
  echo "Las migraciones se aplican fuera del proceso de negocio (decisión 13.70):"
  echo "  DOTNET_ENVIRONMENT=Development dotnet ef database update \\"
  echo "    --project src/FarmaLog.Nucleo.Infrastructure \\"
  echo "    --startup-project $WORKER_DIR"
fi

echo
echo "================= VERIFICACIÓN ================================="
echo "az servicebus queue show -g $RG --namespace-name $SB_NS -n $QUEUE \\"
echo "  --query \"{max:maxDeliveryCount, dup:requiresDuplicateDetection, ventana:duplicateDetectionHistoryTimeWindow, lock:lockDuration}\""