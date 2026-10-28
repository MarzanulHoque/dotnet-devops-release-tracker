# ==========================================
# Stage 1: Build & Publish
# Uses .NET 8.0 SDK on Alpine Linux (~800MB)
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy project files first for Docker layer caching
COPY ["DotnetProject.Api/DotnetProject.Api.csproj", "DotnetProject.Api/"]
COPY ["DotnetProject.Core/DotnetProject.Core.csproj", "DotnetProject.Core/"]

# Restore NuGet dependencies (cached unless packages change)
RUN dotnet restore "DotnetProject.Api/DotnetProject.Api.csproj"

# Copy remaining source code
COPY . .

# Compile and publish optimized Release binaries to /app/publish
WORKDIR /src/DotnetProject.Api
RUN dotnet publish "DotnetProject.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# Stage 2: Runtime Environment
# Uses lightweight ASP.NET 8.0 Alpine (~100MB)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Expose port 5050 and configure Kestrel to bind to it (defaults to 8080 in .NET 8)
EXPOSE 5050
ENV ASPNETCORE_URLS=http://+:5050

# Copy compiled output from the build stage
COPY --from=build /app/publish .

# Start the Web API microservice
ENTRYPOINT ["dotnet", "DotnetProject.Api.dll"]
