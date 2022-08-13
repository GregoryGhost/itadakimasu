FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Itadakimasu.Core.Protos/Itadakimasu.Core.Protos.csproj", "Itadakimasu.Core.Protos/"]
COPY ["Itadakimasu.API.Order/Itadakimasu.API.Order.csproj", "Itadakimasu.API.Order/"]
RUN dotnet restore "Itadakimasu.API.Order/Itadakimasu.API.Order.csproj"
COPY . .
WORKDIR "/src/Itadakimasu.API.Order"
RUN dotnet build "Itadakimasu.API.Order.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Itadakimasu.API.Order.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Itadakimasu.API.Order.dll"]