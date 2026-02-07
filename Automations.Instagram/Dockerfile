#docker build -t jtauanpm/automations-instagram:1.0.x -f Dockerfile . 
#docker tag jtauanpm/automations-instagram:1.0.x jtauanpm/automations-instagram:latest
#docker push jtauanpm/automations-instagram:1.0.x
#docker push jtauanpm/automations-instagram:latest

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Automations.Instagram.csproj", "./"]
RUN dotnet restore "Automations.Instagram.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "./Automations.Instagram.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Automations.Instagram.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Automations.Instagram.dll"]
