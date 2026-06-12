FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Firmeza.Web/Firmeza.Web.csproj Firmeza.Web/
RUN dotnet restore Firmeza.Web/Firmeza.Web.csproj
COPY Firmeza.Web/ Firmeza.Web/
WORKDIR /src/Firmeza.Web
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Firmeza.Web.dll"]
