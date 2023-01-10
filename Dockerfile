#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
ARG VERSION
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV app_version=$VERSION
RUN echo $VERSION

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["WsAdminResidentes.csproj", "."]
RUN dotnet restore "./WsAdminResidentes.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WsAdminResidentes.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WsAdminResidentes.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WsAdminResidentes.dll"]