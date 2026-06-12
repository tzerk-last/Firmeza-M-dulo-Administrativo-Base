FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test
WORKDIR /src
COPY . .
RUN dotnet restore Firmeza.Tests/Firmeza.Tests.csproj
RUN dotnet test Firmeza.Tests/Firmeza.Tests.csproj --no-restore --verbosity normal
