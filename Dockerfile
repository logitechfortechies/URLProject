# ----
# Stage 1: Build the application
# ----
# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
# It now correctly looks for your UrlShortenerApi.csproj file
COPY UrlShortenerApi/UrlShortenerApi.csproj UrlShortenerApi/
RUN dotnet restore UrlShortenerApi/UrlShortenerApi.csproj

# Copy the rest of the source code and build
COPY UrlShortenerApi/ UrlShortenerApi/
WORKDIR /src/UrlShortenerApi
RUN dotnet publish -c Release -o /app/publish

# ----
# Stage 2: Create the final runtime image
# ----
# Use the smaller ASP.NET runtime image for the final product
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080 (Render's default for .NET)
EXPOSE 8080

# Entry point to run the application
# This now correctly uses your project's DLL file
ENTRYPOINT ["dotnet", "UrlShortenerApi.dll"]