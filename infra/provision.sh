#!/usr/bin/env bash
#
# FarmaLog — aprovisionamiento de la rebanada 1 (walking skeleton).
# Crea: Service Bus Standard + cola, Azure SQL (free offer) + reglas de firewall.
# El resource group ya existe (se creó para poder scopear el budget alert).
#
# Uso:          bash infra/provision.sh
# Borrar todo:  az group delete --name rg-farmalog --yes --no-wait
#
# NO contiene secretos. La password del admin de SQL se pide por consola.

set -euo pipefail

# ---------------------------------------------------------------------------
# Parámetros
# ---------------------------------------------------------------------------
LOCATION="chilecentral"            # región de los RECURSOS
RG="rg-farmalog"

SB_NS="sb-farmalog-$(openssl rand -hex 3)"       # el namespace es un nombre DNS global
QUEUE="registrar-solicitud-ingreso"

SQL_SERVER="sql-farmalog-$(openssl rand -hex 3)" # el servidor también es DNS global
SQL_DB="farmalog-nucleo"
SQL_ADMIN="farmalogadmin"

# ---------------------------------------------------------------------------
# Chequeos previos
# ---------------------------------------------------------------------------
echo "==> Suscripción activa:"
az account show --query "{nombre:name, id:id}" -o table

read -rp "¿Es la suscripción correcta? [s/N] " CONFIRMA
[[ "$CONFIRMA" == "s" || "$CONFIRMA" == "S" ]] || { echo "Abortado."; exit 1; }

read -rsp "Password del admin de SQL (min 8, may+min+numero+simbolo): " SQL_PASSWORD; echo

# ---------------------------------------------------------------------------
# Resource group — ya existe. Su location es solo metadata.
# ---------------------------------------------------------------------------
if az group show --name "$RG" -o none 2>/dev/null; then
  echo "==> El resource group $RG ya existe. No se toca."
else
  echo "==> Creando resource group $RG en $LOCATION"
  az group create --name "$RG" --location "$LOCATION" -o none
fi

# ---------------------------------------------------------------------------
# Service Bus
# ---------------------------------------------------------------------------
echo "==> Creando namespace $SB_NS (Standard) en $LOCATION"
az servicebus namespace create \
  --resource-group "$RG" \
  --name "$SB_NS" \
  --location "$LOCATION" \
  --sku Standard \
  -o none

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

# ---------------------------------------------------------------------------
# Azure SQL — free offer, serverless, auto-pause
# ---------------------------------------------------------------------------
echo "==> Creando servidor lógico $SQL_SERVER"
az sql server create \
  --resource-group "$RG" \
  --name "$SQL_SERVER" \
  --location "$LOCATION" \
  --admin-user "$SQL_ADMIN" \
  --admin-password "$SQL_PASSWORD" \
  -o none

echo "==> Creando base $SQL_DB (free offer, AutoPause)"
echo "    Si esto falla, el free offer no está en $LOCATION. Ver plan B en el README."
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

# ---------------------------------------------------------------------------
# Firewall
# ---------------------------------------------------------------------------
echo "==> Regla: permitir servicios de Azure (0.0.0.0 es el flag mágico, no 'internet')"
az sql server firewall-rule create \
  --resource-group "$RG" --server "$SQL_SERVER" \
  --name permitir-servicios-azure \
  --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0 \
  -o none

MI_IP=$(curl -s https://api.ipify.org)
echo "==> Regla: mi máquina ($MI_IP)"
az sql server firewall-rule create \
  --resource-group "$RG" --server "$SQL_SERVER" \
  --name mi-maquina \
  --start-ip-address "$MI_IP" --end-ip-address "$MI_IP" \
  -o none

# ---------------------------------------------------------------------------
# Salida
# ---------------------------------------------------------------------------
SB_CONN=$(az servicebus namespace authorization-rule keys list \
  --resource-group "$RG" --namespace-name "$SB_NS" \
  --name RootManageSharedAccessKey --query primaryConnectionString -o tsv)

SQL_CONN="Server=tcp:${SQL_SERVER}.database.windows.net,1433;Initial Catalog=${SQL_DB};User ID=${SQL_ADMIN};Password=${SQL_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;"

echo
echo "================= NOMBRES GENERADOS (guardalos) ================="
echo "SB_NS=$SB_NS"
echo "SQL_SERVER=$SQL_SERVER"
echo
echo "================= CONNECTION STRINGS ==========================="
echo "ConnectionStrings:ServiceBus"
echo "$SB_CONN"
echo
echo "ConnectionStrings:NucleoDb"
echo "$SQL_CONN"
echo
echo "Cargalas desde src/FarmaLog.Nucleo.Worker con:"
echo "  dotnet user-secrets set \"ConnectionStrings:ServiceBus\" \"...\""
echo "  dotnet user-secrets set \"ConnectionStrings:NucleoDb\" \"...\""
echo
echo "Verificación de la cola:"
echo "  az servicebus queue show -g $RG --namespace-name $SB_NS -n $QUEUE \\"
echo "    --query \"{max:maxDeliveryCount, dup:requiresDuplicateDetection, ventana:duplicateDetectionHistoryTimeWindow, lock:lockDuration}\""