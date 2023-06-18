FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["Domain.Core/Domain.Core.csproj", "Domain.Core/"]
RUN dotnet restore "Domain.Core/Domain.Core.csproj"

COPY ["Infra.Data.Core/Infra.Data.Core.csproj", "Infra.Data.Core/"]
RUN dotnet restore "Infra.Data.Core/Infra.Data.Core.csproj"

COPY ["Services.Hosting/Services.Hosting.csproj", "Services.Hosting/"]
RUN dotnet restore "Services.Hosting/Services.Hosting.csproj"

COPY ["Outbox/Outbox.Domain/Outbox.Domain.csproj", "Outbox/Outbox.Domain/"]
RUN dotnet restore "Outbox/Outbox.Domain/Outbox.Domain.csproj"

COPY ["Outbox/Outbox.Application/Outbox.Application.csproj", "Outbox/Outbox.Application/"]
RUN dotnet restore "Outbox/Outbox.Application/Outbox.Application.csproj"

COPY ["Outbox/Outbox.Infra.Data/Outbox.Infra.Data.csproj", "Outbox/Outbox.Infra.Data/"]
RUN dotnet restore "Outbox/Outbox.Infra.Data/Outbox.Infra.Data.csproj"

COPY ["Outbox/Outbox.Services.Api/Outbox.Services.Api.csproj", "Outbox/Outbox.Services.Api/"]
RUN dotnet restore "Outbox/Outbox.Services.Api/Outbox.Services.Api.csproj"

COPY . .
WORKDIR "/src/Outbox/Outbox.Services.Api"
RUN dotnet build "Outbox.Services.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Outbox.Services.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Outbox.Services.Api.dll"]
