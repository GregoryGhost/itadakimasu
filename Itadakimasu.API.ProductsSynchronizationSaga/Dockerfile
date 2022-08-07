FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Itadakimasu.API.ProductsSynchronizationSaga.csproj", "Itadakimasu.API.ProductsSynchronizationSaga/"]
RUN dotnet restore "Itadakimasu.API.ProductsSynchronizationSaga/Itadakimasu.API.ProductsSynchronizationSaga.csproj"

COPY . Itadakimasu.API.ProductsSynchronizationSaga/.
RUN dotnet publish -c Release -o /app Itadakimasu.API.ProductsSynchronizationSaga/Itadakimasu.API.ProductsSynchronizationSaga.csproj 

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS publish
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Itadakimasu.API.ProductsSynchronizationSaga.dll"]