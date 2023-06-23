#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 7239
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "./"]
COPY ["NuGet.Config", "TCG.AuthenticationService.API/"]
COPY ["NuGet.Config", "TCG.AuthenticationService.Application/"]
COPY ["NuGet.Config", "TCG.AuthenticationService.Domain/"]
COPY ["NuGet.Config", "TCG.AuthenticationService.Persistence/"]
COPY ["TCG.AuthenticationService.API/TCG.AuthenticationService.API.csproj", "TCG.AuthenticationService.API/"]
COPY ["TCG.AuthenticationService.Application/TCG.AuthenticationService.Application.csproj", "TCG.AuthenticationService.Application/"]
COPY ["TCG.AuthenticationService.Domain/TCG.AuthenticationService.Domain.csproj", "TCG.AuthenticationService.Domain/"]
COPY ["TCG.AuthenticationService.Persistence/TCG.AuthenticationService.Persistence.csproj", "TCG.AuthenticationService.Persistence/"]
RUN dotnet restore "TCG.AuthenticationService.API/TCG.AuthenticationService.API.csproj"
RUN dotnet restore "TCG.AuthenticationService.Application/TCG.AuthenticationService.Application.csproj"
RUN dotnet restore "TCG.AuthenticationService.Domain/TCG.AuthenticationService.Domain.csproj"
RUN dotnet restore "TCG.AuthenticationService.Persistence/TCG.AuthenticationService.Persistence.csproj"
COPY . .
WORKDIR "/src/TCG.AuthenticationService.API"
RUN dotnet build "TCG.AuthenticationService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TCG.AuthenticationService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TCG.AuthenticationService.API.dll"]