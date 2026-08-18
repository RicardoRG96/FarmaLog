# ---------- etapa de build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero SOLO los .csproj: es la capa que casi nunca cambia.
COPY src/FarmaLog.Nucleo.Domain/FarmaLog.Nucleo.Domain.csproj                 src/FarmaLog.Nucleo.Domain/
COPY src/FarmaLog.Nucleo.Application/FarmaLog.Nucleo.Application.csproj       src/FarmaLog.Nucleo.Application/
COPY src/FarmaLog.Nucleo.Infrastructure/FarmaLog.Nucleo.Infrastructure.csproj src/FarmaLog.Nucleo.Infrastructure/
COPY src/FarmaLog.Nucleo.Worker/FarmaLog.Nucleo.Worker.csproj                 src/FarmaLog.Nucleo.Worker/

RUN dotnet restore src/FarmaLog.Nucleo.Worker/FarmaLog.Nucleo.Worker.csproj

# Ahora el codigo: la capa que cambia en cada commit.
COPY src/ src/
RUN dotnet publish src/FarmaLog.Nucleo.Worker/FarmaLog.Nucleo.Worker.csproj \
    -c Release --no-restore -o /app/publish

# ---------- etapa de ejecucion ----------
# runtime, NO aspnet: este servicio no sirve HTTP.
# Debian (bookworm/trixie-slim), NO alpine ni chiseled: incluye tzdata,
# que RelojDelSistema necesita para resolver America/Santiago.
LABEL org.opencontainers.image.source="https://github.com/RicardoRG96/FarmaLog"
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Usuario no-root definido por la imagen base (APP_UID=1654).
USER $APP_UID

ENTRYPOINT ["dotnet", "FarmaLog.Nucleo.Worker.dll"]