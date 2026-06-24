FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.sln .
COPY BookingSystem.API/*.csproj BookingSystem.API/
RUN dotnet restore

COPY . .
RUN dotnet publish BookingSystem.API/BookingSystem.API.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "BookingSystem.API.dll"]
