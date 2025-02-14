# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/hosts/LeadManagerNewlyCreatedLeads.Consumer/LeadManagerNewlyCreatedLeads.Consumer.csproj", "src/hosts/LeadManagerNewlyCreatedLeads.Consumer/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.MessageContracts/CrossCutting.MessageContracts.csproj", "src/libs/Cross-cutting/Cross-cutting.MessageContracts/"]
COPY ["src/libs/Domain/Domain.csproj", "src/libs/Domain/"]
COPY ["src/libs/Shared/Shared.csproj", "src/libs/Shared/"]
COPY ["src/hosts/LeadManager.BackendServices.ConsumersCommon/LeadManager.BackendServices.Consumers.Common.csproj", "src/hosts/LeadManager.BackendServices.ConsumersCommon/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Messaging/CrossCutting.Messaging.csproj", "src/libs/Cross-cutting/Cross-cutting.Messaging/"]
COPY ["src/libs/Cross-cutting/Cross-cutting.Serialization.ProtoBuf/CrossCutting.Serialization.ProtoBuf.csproj", "src/libs/Cross-cutting/Cross-cutting.Serialization.ProtoBuf/"]
RUN dotnet restore "./src/hosts/LeadManagerNewlyCreatedLeads.Consumer/LeadManagerNewlyCreatedLeads.Consumer.csproj"
COPY . .
WORKDIR "/src/src/hosts/LeadManagerNewlyCreatedLeads.Consumer"
RUN dotnet build "./LeadManagerNewlyCreatedLeads.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LeadManagerNewlyCreatedLeads.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LeadManagerNewlyCreatedLeads.Consumer.dll"]