FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["init_migrations.sh", "Itadakimasu.API.Products/"]
COPY ["Itadakimasu.Core.DAL/Itadakimasu.Core.DAL.csproj", "Itadakimasu.Core.DAL/"]
COPY ["Itadakimasu.Core.Protos/Itadakimasu.Core.Protos.csproj", "Itadakimasu.Core.Protos/"]
COPY ["Itadakimasu.Products.DAL/Itadakimasu.Products.DAL.csproj", "Itadakimasu.Products.DAL/"]
COPY ["Itadakimasu.Products.Migrations/Itadakimasu.Products.Migrations.csproj", "Itadakimasu.Products.Migrations/"]
COPY ["Itadakimasu.API.Products/Itadakimasu.API.Products.csproj", "Itadakimasu.API.Products/"]
RUN dotnet restore "Itadakimasu.API.Products/Itadakimasu.API.Products.csproj"
COPY . .
WORKDIR "/src/Itadakimasu.API.Products"
RUN dotnet build "Itadakimasu.API.Products.csproj" -c Release -o /app/build
RUN dotnet tool install --global dotnet-ef

RUN chmod +x ./init_migrations.sh
CMD /bin/bash ./init_migrations.sh

FROM build AS publish
RUN dotnet publish "Itadakimasu.API.Products.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Itadakimasu.API.Products.dll"]
