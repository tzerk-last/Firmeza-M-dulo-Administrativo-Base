FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Firmeza.Web/Firmeza.Web.csproj Firmeza.Web/
COPY Firmeza.API/Firmeza.API.csproj Firmeza.API/
RUN dotnet restore Firmeza.API/Firmeza.API.csproj

COPY Firmeza.Web/ Firmeza.Web/
COPY Firmeza.API/ Firmeza.API/

WORKDIR /src/Firmeza.API
RUN dotnet publish -c Release -o /app/publish /p:ErrorOnDuplicatePublishOutputFiles=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Firmeza.API.dll"]
