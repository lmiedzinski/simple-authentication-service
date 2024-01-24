FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/SimpleAuthenticationService.WebApi/SimpleAuthenticationService.WebApi.csproj", "src/SimpleAuthenticationService.WebApi/"]
RUN dotnet restore "src/SimpleAuthenticationService.WebApi/SimpleAuthenticationService.WebApi.csproj"
COPY . .
WORKDIR "/src/src/SimpleAuthenticationService.WebApi"
RUN dotnet build "SimpleAuthenticationService.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleAuthenticationService.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleAuthenticationService.WebApi.dll"]
