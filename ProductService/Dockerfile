# SDK image (build için)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyasını kopyala ve restore et
COPY ["ProductService/ProductService.csproj", "ProductService/"]
RUN dotnet restore "ProductService/ProductService.csproj"

# Tüm kodu kopyala
COPY ProductService/. ./ProductService/
WORKDIR /src/ProductService

# Publish
RUN dotnet publish "ProductService.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Publish edilen dosyaları kopyala
COPY --from=build /app/publish .

# Localization klasörünü de kopyala (burada emin oluyoruz)
COPY ProductService/Localization ./Localization

EXPOSE 80

ENTRYPOINT ["dotnet", "ProductService.dll"]
