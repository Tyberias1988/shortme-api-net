FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["shortme-api-net/shortme-api-net.csproj", "shortme-api-net/"]
RUN dotnet restore "shortme-api-net/shortme-api-net.csproj"
COPY . .
WORKDIR "/src/shortme-api-net"
RUN dotnet build "shortme-api-net.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "shortme-api-net.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "shortme-api-net.dll"]
