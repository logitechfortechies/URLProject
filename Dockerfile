# STAGE 1: Build the Vue.js frontend
FROM node:20 AS frontend-build
WORKDIR /app
COPY client/package.json .
COPY client/package-lock.json .
RUN npm install
COPY client/ .
RUN npm run build

# STAGE 2: Build the .NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src
COPY *.sln .
COPY UrlShortenerApi/*.csproj ./UrlShortenerApi/
COPY UrlShortener.Application/*.csproj ./UrlShortener.Application/
COPY UrlShortener.Domain/*.csproj ./UrlShortener.Domain/
COPY UrlShortener.Infrastructure/*.csproj ./UrlShortener.Infrastructure/
RUN dotnet restore "URLProject.sln"
COPY . .
WORKDIR "/src/UrlShortenerApi"
RUN dotnet publish "UrlShortenerApi.csproj" -c Release -o /app/publish

# STAGE 3: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the built backend
COPY --from=backend-build /app/publish .

# Copy the built frontend into the backend's web root folder
COPY --from=frontend-build /app/dist ./wwwroot

EXPOSE 8080
ENTRYPOINT ["dotnet", "UrlShortenerApi.dll"]
