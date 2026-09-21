# See https://aka.ms/customizecontainer to learn how Visual Studio uses this Dockerfile.

# This stage is used when running from Visual Studio in Fast/Debug mode.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Updated: builds the React/Vite production files for the published application.
FROM node:26-alpine AS client-build
WORKDIR /src/resumeapp.client

COPY ["resumeapp.client/package.json", "resumeapp.client/package-lock.json", "./"]
RUN npm ci

COPY ["resumeapp.client/", "./"]
RUN npm run build

# This stage builds the ASP.NET Core application.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ResumeApp.Server/ResumeApp.Server.csproj", "ResumeApp.Server/"]
RUN dotnet restore "./ResumeApp.Server/ResumeApp.Server.csproj"

COPY . .

# Copy Vite's production output into the ASP.NET Core web root before publishing.
COPY --from=client-build /src/resumeapp.client/dist ./ResumeApp.Server/wwwroot

WORKDIR "/src/ResumeApp.Server"
RUN dotnet build "./ResumeApp.Server.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# This stage publishes the complete application.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ResumeApp.Server.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# This is the final production image.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResumeApp.Server.dll"]
