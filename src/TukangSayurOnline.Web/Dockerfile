FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files for restoring dependencies
COPY ["TukangSayurOnline.Web/TukangSayurOnline.Web.csproj", "TukangSayurOnline.Web/"]
COPY ["TukangSayurOnline.Shared/TukangSayurOnline.Shared.csproj", "TukangSayurOnline.Shared/"]
RUN dotnet restore "TukangSayurOnline.Web/TukangSayurOnline.Web.csproj"

# Copy all source files
COPY . .

WORKDIR "/src/TukangSayurOnline.Web"
RUN dotnet publish "TukangSayurOnline.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TukangSayurOnline.Web.dll"]
