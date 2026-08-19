#!/usr/bin/env bash
#
# FarmaLog — apagado selectivo de fin de jornada.
#
# CRITERIO: se borra un recurso solo si su costo corre con el reloj Y su
# recreación es barata. Los dos ejes, no uno.
#
#   | Recurso            | Costo existiendo | Costo de recrear | Se borra |
#   |--------------------|------------------|------------------|----------|
#   | Service Bus Std.   | ~US$10/mes       | 2-4 min          | SÍ       |
#   | Azure SQL free     | US$0             | hasta 1 hora     | NO       |
#   | Container Apps env | US$0 (*)         | ~30 min          | NO       |
#   | Container App      | US$0 (*)         | ~1 min           | NO       |
#
# (*) Con min-replicas 0, el plan de consumo no cobra uso mientras la app está
#     escalada a cero, y el grant mensual gratuito (180.000 vCPU-s / 360.000
#     GiB-s por suscripción) cubre varias veces el consumo activo de este
#     proyecto. Verificado en la página de precios de Azure Container Apps.
#
# ⚠️ CAMBIO RESPECTO A LA VERSIÓN ANTERIOR: este script YA NO borra Container
#    Apps. El criterio "borro lo que cobra por hora" siempre estuvo bien; lo
#    que estaba mal era la clasificación de Container Apps bajo ese criterio.
#    Es el mismo argumento que salva a Azure SQL: borrar el recurso barato con
#    la penalización más alta es el peor negocio disponible.
#
# Para borrar TODO (fin de proyecto, o para probar el arranque desde cero):
#   az group delete --name rg-farmalog --yes --no-wait
#   ⚠️ Eso paga la penalización de una hora del free offer de SQL Y los ~30
#      minutos del entorno de Container Apps.
#
# Uso: bash infra/teardown.sh

set -euo pipefail

RG="rg-farmalog"
ACA_APP="ca-nucleo-worker"
WORKER_DIR="src/FarmaLog.Nucleo.Worker"
ENV_FILE=".env.local"
SECRETS_FILE=".secrets-comandos.local"

az account show -o none 2>/dev/null \
  || { echo "No hay sesión de Azure. Corré 'az login --use-device-code' primero."; exit 1; }

echo "==> Recursos actuales en $RG:"
az resource list -g "$RG" --query "[].{nombre:name, tipo:type}" -o table

echo
echo "Se borra SOLO el namespace de Service Bus."
echo "Sobreviven: Azure SQL, el entorno de Container Apps y $ACA_APP."
read -rp "¿Continuar? [s/N] " CONFIRMA
[[ "$CONFIRMA" == "s" || "$CONFIRMA" == "S" ]] || { echo "Abortado."; exit 1; }

# --- Service Bus: lo único que cobra por hora exista o no tráfico -----------
for NS in $(az servicebus namespace list -g "$RG" --query "[].name" -o tsv 2>/dev/null || true); do
  echo "==> Borrando namespace $NS"
  az servicebus namespace delete -g "$RG" -n "$NS" -o none
done

# ---------------------------------------------------------------------------
# Invalidar la configuración que acaba de quedar caduca.
#
# La connection string de Service Bus apunta a un namespace inexistente: si
# sobrevive, el worker arranca y falla con un error de red o de credenciales
# en vez de decir "falta configuración". Ausencia es un error legible;
# valor caduco es una trampa.
#
# La de SQL NO se toca: ese servidor sigue vivo y sigue siendo válida.
# ---------------------------------------------------------------------------
if [[ -f "$ENV_FILE" ]]; then
  grep -v '^ConnectionStrings__ServiceBus=' "$ENV_FILE" > "$ENV_FILE.tmp" || true
  mv "$ENV_FILE.tmp" "$ENV_FILE"
  chmod 600 "$ENV_FILE"
  echo "==> $ENV_FILE: eliminada la entrada de Service Bus (la de SQL sigue válida)"
fi

rm -f "$SECRETS_FILE"

if command -v dotnet >/dev/null 2>&1 && [[ -d "$WORKER_DIR" ]]; then
  dotnet user-secrets remove "ConnectionStrings:ServiceBus" --project "$WORKER_DIR" >/dev/null 2>&1 || true
  echo "==> user-secrets: eliminada la entrada de Service Bus"
fi

# ---------------------------------------------------------------------------
# ASIMETRÍA DECLARADA, no olvidada.
#
# El secret 'sb-conn' del Container App también quedó caduco, y NO se retira.
# La razón: el peligro de una configuración caduca es que alguien la consuma
# por accidente. .env.local y los user-secrets los lee un proceso local en
# cualquier momento, sin nada que lo detenga. El Container App, en cambio, no
# puede hacer nada con esa cadena: KEDA no encuentra la cola, la app se queda
# en cero réplicas y no procesa. El recurso que necesitaría para equivocarse
# no existe.
#
# Retirarlo además exigiría desplegar una revisión que no referencie el secret
# antes de poder borrarlo — más maquinaria que el riesgo que evita.
# provision.sh lo repone antes de que haya una cola a la que atender.
# ---------------------------------------------------------------------------
if az containerapp show -g "$RG" -n "$ACA_APP" -o none 2>/dev/null; then
  echo
  echo "⚠️  $ACA_APP sigue en pie y su config de Service Bus quedó caduca."
  echo "    No va a escalar hasta que corras provision.sh (que la repone)."
fi

echo
echo "==> Listo. Sobrevive:"
az resource list -g "$RG" --query "[].{nombre:name, tipo:type}" -o table
echo
echo "Mañana: bash infra/provision.sh"
echo "Crea el namespace nuevo y repone su configuración en los TRES destinos:"
echo ".env.local, user-secrets y los secrets del Container App."