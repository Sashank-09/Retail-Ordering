FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY ["UrbanBites.API/UrbanBites.API.csproj", "UrbanBites.API/"]
COPY ["UrbanBites.Application/UrbanBites.Application.csproj", "UrbanBites.Application/"]
COPY ["UrbanBites.Domain/UrbanBites.Domain.csproj", "UrbanBites.Domain/"]
COPY ["UrbanBites.Infrastructure/UrbanBites.Infrastructure.csproj", "UrbanBites.Infrastructure/"]
RUN dotnet restore "UrbanBites.API/UrbanBites.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/app/UrbanBites.API"
RUN dotnet build "UrbanBites.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "UrbanBites.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrbanBites.API.dll"]
