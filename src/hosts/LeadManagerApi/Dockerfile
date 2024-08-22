#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/hosts/LeadManagerApi/LeadManagerApi.csproj", "src/hosts/LeadManagerApi/"]
COPY ["src/libs/Application/Application.csproj", "src/libs/Application/"]
COPY ["src/libs/Domain/Domain.csproj", "src/libs/Domain/"]
COPY ["src/libs/Shared/Shared.csproj", "src/libs/Shared/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Caching/CrossCutting.Caching.csproj", "src/libs/Cross-cutting/Cross-cutting.Caching/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Serialization.ProtoBuf/CrossCutting.Serialization.ProtoBuf.csproj", "src/libs/Cross-cutting/Cross-cutting.Serialization.ProtoBuf/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Csv/CrossCutting.Csv.csproj", "src/libs/Cross-cutting/Cross-cutting.Csv/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.FileStorage/CrossCutting.FileStorage.csproj", "src/libs/Cross-cutting/Cross-cutting.FileStorage/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Logging/CrossCutting.Logging.csproj", "src/libs/Cross-cutting/Cross-cutting.Logging/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.MessageContracts/CrossCutting.MessageContracts.csproj", "src/libs/Cross-cutting/Cross-cutting.MessageContracts/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Messaging/CrossCutting.Messaging.csproj", "src/libs/Cross-cutting/Cross-cutting.Messaging/"]
COPY ["src/libs/Integrations/ViaCep.ServiceClient/ViaCep.ServiceClient.csproj", "src/libs/Integrations/ViaCep.ServiceClient/"]
COPY ["src/libs/Infrastructure/Infrastructure.csproj", "src/libs/Infrastructure/"]
RUN dotnet restore "src/hosts/LeadManagerApi/LeadManagerApi.csproj"
COPY . .
WORKDIR "/src/src/hosts/LeadManagerApi"
RUN dotnet build "LeadManagerApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LeadManagerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LeadManagerApi.dll"]