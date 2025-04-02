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

# Install Python3, pip, and create a virtual environment
RUN apt-get update && apt-get install -y python3 python3-pip python3-venv && \
    python3 -m venv /opt/venv && \
    /opt/venv/bin/pip install --upgrade pip && \
    /opt/venv/bin/pip install yt-dlp

# Install Playwright and its dependencies
RUN apt-get install -y \
    wget \
    curl \
    gnupg \
    libx11-dev \
    libxcomposite1 \
    libxdamage1 \
    libxrandr2 \
    libxtst6 \
    libnss3 \
    libatk-bridge2.0-0 \
    libatk1.0-0 \
    libnspr4 \
    libgbm-dev \
    libxss1 \
    libgdk-pixbuf2.0-0 \
    libpangocairo-1.0-0 \
    libgdk-pixbuf2.0-0 \
    fonts-liberation \
    libappindicator3-1 \
    libasound2 \
    lsb-release \
    xdg-utils \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*

# Install Node.js and npm
RUN curl -sL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

# Install Playwright's required browsers
RUN npm init playwright@latest && \
    npx playwright install

# Ensure the virtual environment is used
ENV PATH="/opt/venv/bin:$PATH"

# Copy files from the publish stage
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS="http://*:8080"

# Start application
ENTRYPOINT ["dotnet", "Api.dll"]