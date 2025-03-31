# Base image for production
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["ReportHub.API/ReportHub.API.csproj", "ReportHub.API/"]
COPY ["ReportHub.Application/ReportHub.Application.csproj", "ReportHub.Application/"]
COPY ["ReportHub.Domain/ReportHub.Domain.csproj", "ReportHub.Domain/"]

# Restore dependencies
RUN dotnet restore "./ReportHub.API/ReportHub.API.csproj"

# Copy all files and build
COPY . .
WORKDIR "/src/ReportHub.API"
RUN dotnet build "./ReportHub.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReportHub.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure app listens to Heroku's assigned PORT
ENV ASPNETCORE_URLS=http://+:8080

CMD ["dotnet", "ReportHub.API.dll"]