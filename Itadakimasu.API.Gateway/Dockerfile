FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Itadakimasu.Core.Protos/Itadakimasu.Core.Protos.csproj", "Itadakimasu.Core.Protos/"]
COPY ["Itadakimasu.API.Gateway/Itadakimasu.API.Gateway.csproj", "Itadakimasu.API.Gateway/"]
RUN dotnet restore "Itadakimasu.API.Gateway/Itadakimasu.API.Gateway.csproj"
COPY . .
WORKDIR "/src/Itadakimasu.API.Gateway"
RUN dotnet build "Itadakimasu.API.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Itadakimasu.API.Gateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Itadakimasu.API.Gateway.dll"]