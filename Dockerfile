# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY FileVault/*.csproj ./FileVault/
RUN dotnet restore

# copy everything else and build app
COPY FileVault/. ./FileVault/
WORKDIR /source/FileVault
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:3000

COPY --from=build /app ./

EXPOSE 3000

CMD ["dotnet", "FileVault.dll"]