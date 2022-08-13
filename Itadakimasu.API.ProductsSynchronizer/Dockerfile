FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Itadakimasu.Core.Protos/Itadakimasu.Core.Protos.csproj", "Itadakimasu.Core.Protos/"]
COPY ["Itadakimasu.API.ProductsSynchronizer/Itadakimasu.API.ProductsSynchronizer.csproj", "Itadakimasu.API.ProductsSynchronizer/"]
RUN dotnet restore "Itadakimasu.API.ProductsSynchronizer/Itadakimasu.API.ProductsSynchronizer.csproj"
COPY . .
WORKDIR "/src/Itadakimasu.API.ProductsSynchronizer"
RUN dotnet build "Itadakimasu.API.ProductsSynchronizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Itadakimasu.API.ProductsSynchronizer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Itadakimasu.API.ProductsSynchronizer.dll"]