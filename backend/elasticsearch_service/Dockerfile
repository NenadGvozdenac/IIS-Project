FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5008
EXPOSE 5009

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/elasticsearch_service/elasticsearch_service.csproj", "backend/elasticsearch_service/"]
RUN dotnet restore "backend/elasticsearch_service/elasticsearch_service.csproj"
COPY . .
WORKDIR "/src/backend/elasticsearch_service"
RUN dotnet build "elasticsearch_service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "elasticsearch_service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "elasticsearch_service.dll"]