FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY XBullet.ScoreService.csproj ./
RUN dotnet restore XBullet.ScoreService.csproj

COPY . ./
RUN dotnet publish XBullet.ScoreService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV SCORE_DATA_DIR=/app/data

COPY --from=build /app/publish ./
RUN mkdir -p /app/data

EXPOSE 8080
ENTRYPOINT ["dotnet", "XBullet.ScoreService.dll"]