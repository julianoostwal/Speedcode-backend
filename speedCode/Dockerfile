FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY speedCode.WebHost/*.csproj ./speedCode.WebHost/
COPY modules/*/*.csproj ./modules/
RUN dotnet restore ./speedCode.WebHost/speedCode.WebHost.csproj

COPY . .

RUN dotnet publish ./speedCode.WebHost/speedCode.WebHost.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "speedCode.WebHost.dll"]
