# Use the official ASP.NET Core runtime as a base image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy the csproj file and restore dependencies
COPY ["RaceDayMVC/RaceDayMVC.csproj", "RaceDayMVC/"]
RUN dotnet restore "./RaceDayMVC/RaceDayMVC.csproj"
# Copy the rest of the files and build
COPY . .
WORKDIR "/src/RaceDayMVC"
RUN dotnet build "./RaceDayMVC.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RaceDayMVC.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RaceDayMVC.dll"]