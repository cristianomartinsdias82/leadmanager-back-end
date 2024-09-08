FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=goodluck
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=goodluck

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["hosts/IAMServer/certs/aspnetapp.pfx", "/https/aspnetapp.pfx"]
COPY ["hosts/IAMServer/IAMServer.csproj", "src/hosts/IAMServer/"]
COPY ["libs/Cross-cutting/Cross-cutting.Monitoring/CrossCutting.Monitoring.csproj", "src/libs/Cross-cutting/Cross-cutting.Monitoring/"]
COPY ["libs/Integrations/ViaCep.ServiceClient/ViaCep.ServiceClient.csproj", "src/libs/Integrations/ViaCep.ServiceClient/"]
COPY ["libs/Shared/Shared.csproj", "src/libs/Shared/"]
COPY ["libs/Cross-cutting/Cross-cutting.Caching/CrossCutting.Caching.csproj", "src/libs/Cross-cutting/Cross-cutting.Caching/"]
COPY ["libs/Cross-cutting/Cross-cutting.Serialization.ProtoBuf/CrossCutting.Serialization.ProtoBuf.csproj", "src/libs/Cross-cutting/Cross-cutting.Serialization.ProtoBuf/"]
COPY ["libs/Cross-cutting/Cross-cutting.FileStorage/CrossCutting.FileStorage.csproj", "src/libs/Cross-cutting/Cross-cutting.FileStorage/"]
COPY ["libs/Cross-cutting/Cross-cutting.Messaging/CrossCutting.Messaging.csproj", "src/libs/Cross-cutting/Cross-cutting.Messaging/"]
COPY ["libs/Cross-cutting/Cross-cutting.Security/CrossCutting.Security.csproj", "src/libs/Cross-cutting/Cross-cutting.Security/"]




RUN dotnet restore "./src/hosts/IAMServer/IAMServer.csproj"
COPY . .
WORKDIR "hosts/IAMServer"
RUN dotnet build "./IAMServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./IAMServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["hosts/IAMServer/certs/aspnetapp.pfx", "/https/aspnetapp.pfx"]

ENTRYPOINT ["dotnet", "IAMServer.dll"]