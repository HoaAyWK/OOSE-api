FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./OpenRequest.Api ./OpenRequest.Api 
COPY . .

WORKDIR /src/OpenRequest.Api
RUN dotnet build -c Release -o /app

FROM build as publish
RUN dotnet publish  -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet OpenRequest.Api.dll