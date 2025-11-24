# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Install CA certificates and MySQL client at build time
RUN apt-get update && \
    apt-get install -y default-mysql-client ca-certificates && \
    rm -rf /var/lib/apt/lists/*

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Product_Config_Customer_v0.csproj", "."]
RUN dotnet restore "./Product_Config_Customer_v0.csproj"
COPY . .
RUN dotnet build "./Product_Config_Customer_v0.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "./Product_Config_Customer_v0.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product_Config_Customer_v0.dll"]
