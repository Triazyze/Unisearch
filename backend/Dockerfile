# Stage 1: Base commune
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

WORKDIR /app

COPY ./src ./src

RUN dotnet restore ./src/UniSearch.Api/UniSearch.Api.csproj


# Stage 2: Développement
FROM base AS dev

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl --fail --silent http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "watch", "--project", "./src/UniSearch.Api/UniSearch.Api.csproj", "run", "--no-launch-profile"]


# Stage 3: Tests
FROM base AS test

COPY ./tests ./tests
COPY ./UniSearch.sln ./

ENTRYPOINT ["dotnet", "test", "UniSearch.sln"]


# Stage 4: Build de production
FROM base AS build

RUN dotnet publish ./src/UniSearch.Api/UniSearch.Api.csproj \
    -c Release \
    -o /app/publish


# Stage 5: Production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS prod

WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl --fail --silent http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "UniSearch.Api.dll"]
