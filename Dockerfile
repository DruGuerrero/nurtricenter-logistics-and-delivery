FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Nurtricenter.Api/Nurtricenter.Api.csproj                      src/Nurtricenter.Api/
COPY src/Nurtricenter.Application/Nurtricenter.Application.csproj      src/Nurtricenter.Application/
COPY src/Nurtricenter.Core/Nurtricenter.Core.csproj                    src/Nurtricenter.Core/
COPY src/Nurtricenter.Infrastructure/Nurtricenter.Infrastructure.csproj src/Nurtricenter.Infrastructure/

RUN dotnet restore src/Nurtricenter.Api/Nurtricenter.Api.csproj

COPY src/ src/
RUN dotnet publish src/Nurtricenter.Api/Nurtricenter.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

# Required at run time (pass via `docker run -e` or compose `environment:`):
#   ConnectionStrings__DefaultConnection   -> PostgreSQL connection string
#   ClinicService__BaseUrl                 -> external clinic API base URL
#   branchCoordinates__latitude            -> starting point latitude
#   branchCoordinates__longitude           -> starting point longitude

ENTRYPOINT ["dotnet", "Nurtricenter.Api.dll"]
