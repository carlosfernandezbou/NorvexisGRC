# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY GRC.csproj ./
RUN dotnet restore GRC.csproj

COPY . ./
RUN dotnet publish GRC.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish ./

COPY --from=build /src/Infrastructure/Seed/Json ./Infrastructure/Seed/Json

COPY wait-for-cosmos.sh /app/wait-for-cosmos.sh
RUN sed -i 's/\r$//' /app/wait-for-cosmos.sh && chmod +x /app/wait-for-cosmos.sh

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["/app/wait-for-cosmos.sh"]
