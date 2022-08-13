FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Itadakimasu.Core.Protos/Itadakimasu.Core.Protos.csproj", "Itadakimasu.Core.Protos/"]
COPY ["Itadakimasu.API.ProductsSynchronizationSaga/Itadakimasu.API.ProductsSynchronizationSaga.csproj", "Itadakimasu.API.ProductsSynchronizationSaga/"]
RUN dotnet restore "Itadakimasu.API.ProductsSynchronizationSaga/Itadakimasu.API.ProductsSynchronizationSaga.csproj"
COPY . .
WORKDIR "/src/Itadakimasu.API.ProductsSynchronizationSaga"
RUN dotnet build "Itadakimasu.API.ProductsSynchronizationSaga.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Itadakimasu.API.ProductsSynchronizationSaga.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Itadakimasu.API.ProductsSynchronizationSaga.dll"]
