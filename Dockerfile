# Prep work for all images.
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 8888

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS restore
WORKDIR /src
COPY . /src
RUN dotnet restore "./fission-dotnet5.sln"

# Specific work for environment image.
FROM restore as build-env
RUN dotnet publish "./fission-dotnet5/fission-dotnet5.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "fission-dotnet5.dll"]
