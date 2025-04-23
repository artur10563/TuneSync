#From /Api folder run this command
#docker build -t api-image -f Dockerfile ..
#docker run -d -p 8080:8080 --name api-image {environment_variables} api-image:latest
#docker run -d -p 8080:8080 --name api-image --env-file {path_to_file} api-image:latest

# Base image with ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files (relative to the Api folder)
COPY ["../Api/Api.csproj", "Api/"]
COPY ["../Application/Application.csproj", "Application/"]
COPY ["../Domain/Domain.csproj", "Domain/"]
COPY ["../Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["../Tests/Tests.csproj", "Tests/"]

# Restore dependencies
RUN dotnet restore "Api/Api.csproj"
RUN dotnet restore "Tests/Tests.csproj"

# Copy remaining source files
COPY .. .

# Build application
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

WORKDIR "/src/Tests"
RUN dotnet build "Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet test "Tests/Tests.csproj" -c $BUILD_CONFIGURATION --no-build

# Publish stage
FROM build AS publish
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app

# Install Python3, pip, and create a virtual environment
RUN apt-get update && apt-get install -y python3 python3-pip python3-venv && \
    python3 -m venv /opt/venv && \
    /opt/venv/bin/pip install --upgrade pip && \
    /opt/venv/bin/pip install yt-dlp


# Ensure the virtual environment is used
ENV PATH="/opt/venv/bin:$PATH"

# Copy files from the publish stage
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS="http://*:8080"

# Start application
ENTRYPOINT ["dotnet", "Api.dll"]