FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5010
EXPOSE 5011

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/elastic_orchestrator_service/elastic_orchestrator_service.csproj", "backend/elastic_orchestrator_service/"]
RUN dotnet restore "backend/elastic_orchestrator_service/elastic_orchestrator_service.csproj"
COPY . .
WORKDIR "/src/backend/elastic_orchestrator_service"
RUN dotnet build "elastic_orchestrator_service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "elastic_orchestrator_service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "elastic_orchestrator_service.dll"]