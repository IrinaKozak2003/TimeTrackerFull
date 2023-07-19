FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TimeTrackerServer/TimeTrackerServer.csproj", "TimeTrackerServer/"]
RUN dotnet restore "TimeTrackerServer/TimeTrackerServer.csproj"
COPY . .
WORKDIR "/src/TimeTrackerServer"
RUN dotnet build "TimeTrackerServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeTrackerServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeTrackerServer.dll"]
