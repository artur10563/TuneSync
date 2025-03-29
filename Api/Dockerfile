#From /Api folder run this command
#docker build -t api-image -f Dockerfile ..

#docker run -d -p 8080:8080 -p 8081:8081 --name api-image -e ConnectionStrings__Default="database_connection" -e FirestoreStorage__Default="gs://project.com" -e FirestoreStorage__DefaultBucket="project.appspot.com" -e 
#YouTubeApi__ApiKey="ytApiKey" -e Auth__TokenUrl="https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=FirebaseKey" -e Auth__RefreshTokenUrl="https://securetoken.googleapis
#.com/v1/token?key=FirebaseKey" -e Auth__Key="FirebaseKey" -e Auth__ValidIssuer="https://securetoken.google.com/https://securetoken.google.com/project" -e 
#Auth__Audience="project" -e FirestoreStorage__ProjectId="project" api-image:latest

# Set environment variables (These can be overridden in deployment settings or in docker-compose file)
#ENV ConnectionStrings__Default="${DATABASE_CONNECTION_STRING}"
#ENV FirestoreStorage__Default="gs://${STORAGE_LINK}"
#ENV FirestoreStorage__DefaultBucket="${BUCKET_NAME}"
#ENV YouTubeApi__ApiKey="${YOUTUBE_KEY}"
#ENV Auth__TokenUrl="https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=${FIREBASE_KEY}"
#ENV Auth__RefreshTokenUrl="https://securetoken.googleapis.com/v1/token?key=${FIREBASE_KEY}"
#ENV Auth__Key="${FIREBASE_KEY}"
#ENV Auth__ValidIssuer="https://securetoken.google.com/${PROJECT_ID}"
#ENV Auth__Audience="${PROJECT_ID}"
#ENV FirestoreStorage__ProjectId="$PROJECT_ID"
#ENV FirebaseConfig__type = Copy_From_Firebase.json
#ENV FirebaseConfig__project_id = Copy_From_Firebase.json
#ENV FirebaseConfig__private_key_id = Copy_From_Firebase.json
#ENV FirebaseConfig__private_key = Copy_From_Firebase.json
#ENV FirebaseConfig__client_email = Copy_From_Firebase.json
#ENV FirebaseConfig__client_id = Copy_From_Firebase.json
#ENV FirebaseConfig__auth_uri = Copy_From_Firebase.json
#ENV FirebaseConfig__token_uri = Copy_From_Firebase.json
#ENV FirebaseConfig__auth_provider_x509_cert_url = Copy_From_Firebase.json
#ENV FirebaseConfig__client_x509_cert_url = Copy_From_Firebase.json
#ENV FirebaseConfig__universe_domain = Copy_From_Firebase.json


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