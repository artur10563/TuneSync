#From /Api folder run this command
#docker build -t api-image -f Dockerfile ..

#docker run -d -p 8080:8080 -p 8081:8081 --name api-image -e ConnectionStrings__Default="database_connection" -e FirestoreStorage__Default="gs://project.com" -e FirestoreStorage__DefaultBucket="project.appspot.com" -e 
#YouTubeApi__ApiKey="ytApiKey" -e Auth__TokenUrl="https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=FirebaseKey" -e Auth__RefreshTokenUrl="https://securetoken.googleapis
#.com/v1/token?key=FirebaseKey" -e Auth__Key="FirebaseKey" -e Auth__ValidIssuer="https://securetoken.google.com/https://securetoken.google.com/project" -e 
#Auth__Audience="project" api-image:latest

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
COPY ../Api/firebase.json /app/Api/firebase.json

ENV ASPNETCORE_URLS="http://*:8080"

# Start application
ENTRYPOINT ["dotnet", "Api.dll"]
