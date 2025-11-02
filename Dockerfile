# ----
# Stage 1: Build the application
# ----
# Use the official .NET 8.0 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the ENTIRE project folder into the build container
COPY UrlShortenerApi/ .

# Run restore on the project file
RUN dotnet restore "UrlShortenerApi.csproj"

# Run publish
RUN dotnet publish "UrlShortenerApi.csproj" -c Release -o /app/publish

# ----
# Stage 2: Create the final runtime image
# ----
# Use the smaller .NET 8.0 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080 (Render's default for .NET)
EXPOSE 8080

# Entry point to run the application
ENTRYPOINT ["dotnet", "UrlShortenerApi.dll"]