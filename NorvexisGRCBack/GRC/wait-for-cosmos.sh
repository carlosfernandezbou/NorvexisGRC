#!/bin/sh
# Espera a que el emulador de Azure Cosmos DB (vNext) esté listo antes de arrancar la API.
# El emulador vNext expone un health probe en el puerto 8080; /ready solo responde 200
# cuando ya acepta operaciones de datos, lo que evita sembrar demasiado pronto.
# (-k por si el endpoint fuese HTTPS con certificado autofirmado; para 8080/HTTP es inocuo.)
set -e

COSMOS_URL="${COSMOS_HEALTH_URL:-http://cosmos:8080/ready}"
MAX_RETRIES="${COSMOS_MAX_RETRIES:-60}"
SLEEP_SECONDS="${COSMOS_RETRY_INTERVAL:-5}"

echo "Esperando al emulador de Cosmos en ${COSMOS_URL} ..."

i=0
until curl -ksf "${COSMOS_URL}" > /dev/null 2>&1; do
    i=$((i + 1))
    if [ "${i}" -ge "${MAX_RETRIES}" ]; then
        echo "Cosmos no respondió tras ${i} intentos; se arranca de todas formas (el seeding puede fallar)."
        break
    fi
    echo "  ...intento ${i}/${MAX_RETRIES}: Cosmos aún no está listo. Reintentando en ${SLEEP_SECONDS}s."
    sleep "${SLEEP_SECONDS}"
done

echo "Arrancando la API GRC."
exec dotnet GRC.dll
