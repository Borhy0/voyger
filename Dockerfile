# =========================
# Build
# =========================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project files first
COPY ["Voyagr.API/Voyagr.API.csproj", "Voyagr.API/"]
COPY ["Voyagr.Application/Voyagr.Application.csproj", "Voyagr.Application/"]
COPY ["Voyagr.Domain/Voyagr.Domain.csproj", "Voyagr.Domain/"]
COPY ["Voyagr.Infrastructure/Voyagr.Infrastructure.csproj", "Voyagr.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Voyagr.API/Voyagr.API.csproj"

# Copy the rest of the source
COPY . .

# Build
WORKDIR "/src/Voyagr.API"

RUN dotnet build "Voyagr.API.csproj" \
    -c Release \
    -o /app/build

# Publish
RUN dotnet publish "Voyagr.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# Runtime
# =========================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# Render will provide PORT
ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "Voyagr.API.dll"]