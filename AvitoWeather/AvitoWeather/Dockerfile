#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

ENV LISTEN_PORT=80
ENV LISTEN_PORT_SSL=443

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AvitoWeather/AvitoWeather.csproj", "AvitoWeather/"]
RUN dotnet restore "AvitoWeather/AvitoWeather.csproj"
COPY . .
WORKDIR "/src/AvitoWeather"
RUN dotnet build "AvitoWeather.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AvitoWeather.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AvitoWeather.dll"]