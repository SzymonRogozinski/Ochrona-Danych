# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./Client/Client.csproj","."]
RUN dotnet restore "./Client/Client.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Client/Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Client/Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","Client.dll"]