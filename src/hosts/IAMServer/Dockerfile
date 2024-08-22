FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=goodluck
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=goodluck

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy the HTTPS certificate
COPY ["src/hosts/IAMServer/certs/aspnetapp.pfx", "/https/aspnetapp.pfx"]
COPY ["src/hosts/IAMServer/IAMServer.csproj", "src/hosts/IAMServer/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Monitoring/CrossCutting.Monitoring.csproj", "src/libs/Cross-cutting/Cross-cutting.Monitoring/"]
COPY ["src/libs/Integrations/ViaCep.ServiceClient/ViaCep.ServiceClient.csproj", "src/libs/Integrations/ViaCep.ServiceClient/"]
COPY ["src/libs/Shared/Shared.csproj", "src/libs/Shared/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Caching/CrossCutting.Caching.csproj", "src/libs/Cross-cutting/Cross-cutting.Caching/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Serialization.ProtoBuf/CrossCutting.Serialization.ProtoBuf.csproj", "src/libs/Cross-cutting/Cross-cutting.Serialization.ProtoBuf/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.FileStorage/CrossCutting.FileStorage.csproj", "src/libs/Cross-cutting/Cross-cutting.FileStorage/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Messaging/CrossCutting.Messaging.csproj", "src/libs/Cross-cutting/Cross-cutting.Messaging/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Security/CrossCutting.Security.csproj", "src/libs/Cross-cutting/Cross-cutting.Security/"]




RUN dotnet restore "./src/hosts/IAMServer/IAMServer.csproj"
COPY . .
WORKDIR "/src/src/hosts/IAMServer"
RUN dotnet build "./IAMServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./IAMServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IAMServer.dll"]