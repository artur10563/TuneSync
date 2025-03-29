#From /Api folder run this command
#docker build -t api-image -f Dockerfile ..
#docker run -d -p 8080:8080 --name api-image {environment_variables} api-image:latest
#docker run -d -p 8080:8080 --name api-image --env-file {path_to_file} api-image:latest

# Base image with ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files (relative to the Api folder)
COPY ["../Api/Api.csproj", "Api/"]
COPY ["../Application/Application.csproj", "Application/"]
COPY ["../Domain/Domain.csproj", "Domain/"]
COPY ["../Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Api/Api.csproj"

# Copy remaining source files
COPY .. .

# Build application
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app

# Copy files from the publish stage
COPY --from=publish /app/publish .

# Make sure firebase.json is copied
#COPY ../Api/firebase.json /app/Api/firebase.json


ENV ASPNETCORE_URLS="http://*:8080"

# Start application
ENTRYPOINT ["dotnet", "Api.dll"]
