#!/usr/bin/env bash
#
# FarmaLog — apagado selectivo de fin de jornada.
#
# Borra SOLO lo que cobra por hora exista o no tráfico:
#   - Service Bus Standard (namespace + cola)
#   - Container Apps y su entorno (cuando existan)
#
# NO toca Azure SQL: con free limit no genera costo, y recrearlo cuesta hasta
# una hora de espera hasta que se libere el slot del free offer. Borrar el
# recurso barato con la penalización más alta es el peor negocio disponible.
#
# Para borrar TODO (fin de proyecto, o para probar el arranque desde cero):
#   az group delete --name rg-farmalog --yes --no-wait
#
# Uso: bash infra/teardown.sh

set -euo pipefail

RG="rg-farmalog"
WORKER_DIR="src/FarmaLog.Nucleo.Worker"
ENV_FILE=".env.local"
SECRETS_FILE=".secrets-comandos.local"

az account show -o none 2>/dev/null \
  || { echo "No hay sesión de Azure. Corré 'az login --use-device-code' primero."; exit 1; }

echo "==> Recursos actuales en $RG:"
az resource list -g "$RG" --query "[].{nombre:name, tipo:type}" -o table

read -rp "¿Borrar Service Bus y Container Apps? (SQL queda intacto) [s/N] " CONFIRMA
[[ "$CONFIRMA" == "s" || "$CONFIRMA" == "S" ]] || { echo "Abortado."; exit 1; }

# --- Container Apps (pueden no existir todavía) -----------------------------
for CA in $(az containerapp list -g "$RG" --query "[].name" -o tsv 2>/dev/null || true); do
  echo "==> Borrando container app $CA"
  az containerapp delete -g "$RG" -n "$CA" --yes -o none
done

for ENV in $(az containerapp env list -g "$RG" --query "[].name" -o tsv 2>/dev/null || true); do
  echo "==> Borrando entorno de Container Apps $ENV"
  az containerapp env delete -g "$RG" -n "$ENV" --yes -o none
done

# --- Service Bus ------------------------------------------------------------
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

echo
echo "==> Listo. Sobrevive:"
az resource list -g "$RG" --query "[].{nombre:name, tipo:type}" -o table
echo
echo "Mañana: bash infra/provision.sh"
echo "El namespace nuevo tendrá otro nombre y otras claves, así que el script"
echo "regenera .env.local y reimprime los comandos de user-secrets."