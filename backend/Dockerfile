# Stage 1: Base commune
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

WORKDIR /app

COPY ./src ./src

RUN dotnet restore ./src/UniSearch.Api/UniSearch.Api.csproj

# Stage 2: Développement
FROM base AS dev

EXPOSE 8080

ENTRYPOINT ["dotnet", "watch", "--project", "./src/UniSearch.Api/UniSearch.Api.csproj", "run"]

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

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS prod

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "UniSearch.Api.dll"]
