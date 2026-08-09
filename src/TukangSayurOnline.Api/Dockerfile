FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files for restoring dependencies efficiently
COPY ["TukangSayurOnline.Api/TukangSayurOnline.Api.csproj", "TukangSayurOnline.Api/"]
RUN dotnet restore "TukangSayurOnline.Api/TukangSayurOnline.Api.csproj"

# Copy all source files
COPY . .

WORKDIR "/src/TukangSayurOnline.Api"
RUN dotnet publish "TukangSayurOnline.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TukangSayurOnline.Api.dll"]
