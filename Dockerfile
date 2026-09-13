FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "KanishkJewellers.csproj"
RUN dotnet publish "KanishkJewellers.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
ENV DATA_DIR=/var/data

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "KanishkJewellers.dll"]
