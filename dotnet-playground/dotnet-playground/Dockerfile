FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["dotnet-playground/dotnet-playground.csproj", "dotnet-playground/"]
RUN dotnet restore "dotnet-playground/dotnet-playground.csproj"
COPY . .
WORKDIR "/src/dotnet-playground"
RUN dotnet build "dotnet-playground.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dotnet-playground.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnet-playground.dll"]
