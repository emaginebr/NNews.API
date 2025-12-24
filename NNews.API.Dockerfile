# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["NNews.API/NNews.API.csproj", "NNews.API/"]
COPY ["NNews.Application/NNews.Application.csproj", "NNews.Application/"]
COPY ["NNews.Domain/NNews.Domain.csproj", "NNews.Domain/"]
COPY ["NNews.Dtos/NNews.Dtos.csproj", "NNews.Dtos/"]
COPY ["NNews.Infra/NNews.Infra.csproj", "NNews.Infra/"]
COPY ["NNews.Infra.Interfaces/NNews.Infra.Interfaces.csproj", "NNews.Infra.Interfaces/"]
COPY ["Lib/", "Lib/"]

# Restore dependencies
RUN dotnet restore "NNews.API/NNews.API.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/NNews.API"
RUN dotnet build "NNews.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "NNews.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create necessary directories with correct permissions
RUN mkdir -p /app/logs /app/certs && \
    chmod 755 /app/logs && \
    chmod 755 /app/certs

# Expose ports
EXPOSE 8080
EXPOSE 8443

# Copy published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080;https://+:8443
ENV ASPNETCORE_ENVIRONMENT=Docker
ENV CERTIFICATE_PATH="/app/certs/certificate.pfx"
ENV CERTIFICATE_PASSWORD="pikpro6"

# Run the application
ENTRYPOINT ["dotnet", "NNews.API.dll"]
