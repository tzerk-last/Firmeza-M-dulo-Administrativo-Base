FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Firmeza.Client/Firmeza.Client.csproj Firmeza.Client/
RUN dotnet restore Firmeza.Client/Firmeza.Client.csproj
COPY Firmeza.Client/ Firmeza.Client/
WORKDIR /src/Firmeza.Client
RUN dotnet publish -c Release -o /app/publish

FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY Firmeza.Client/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
