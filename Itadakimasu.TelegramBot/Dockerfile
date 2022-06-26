FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Itadakimasu.TelegramBot/Itadakimasu.TelegramBot.csproj", "Itadakimasu.TelegramBot/"]
RUN dotnet restore "Itadakimasu.TelegramBot/Itadakimasu.TelegramBot.csproj"
COPY . .
WORKDIR "/src/Itadakimasu.TelegramBot"
RUN dotnet build "Itadakimasu.TelegramBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Itadakimasu.TelegramBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Itadakimasu.TelegramBot.dll"]
