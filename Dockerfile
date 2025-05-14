FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["KPI.sln", "./"]
COPY ["src/KPISolution.csproj", "src/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/src"
RUN dotnet build "KPISolution.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KPISolution.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KPISolution.dll"] 