FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM node:slim AS frontend
WORKDIR /src
COPY ["Frontend", "."]
RUN npm install
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["App/App.csproj", "App/"]
COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["Application/Application.csproj", "Application/"]
RUN dotnet restore "App/App.csproj"
COPY . .
WORKDIR "/src/App"
COPY --from=frontend ["src/dist/", "App/wwwroot/"]
RUN dotnet build "App.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet test --no-build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.dll"]
