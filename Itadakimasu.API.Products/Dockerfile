FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Itadakimasu.API.Products/Itadakimasu.API.Products.csproj", "Itadakimasu.API.Products/"]
RUN dotnet restore "Itadakimasu.API.Products/Itadakimasu.API.Products.csproj"
COPY . .
WORKDIR "/src/Itadakimasu.API.Products"
RUN dotnet build "Itadakimasu.API.Products.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Itadakimasu.API.Products.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Itadakimasu.API.Products.dll"]
