# مرحلة القاعدة (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# مرحلة البناء (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

# استعادة الحزم وبناء المشروع
RUN dotnet restore "LicenseApiProject.csproj"
RUN dotnet publish "LicenseApiProject.csproj" -c Release -o /app/publish

# مرحلة النشر (Final)
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# أمر تشغيل التطبيق
ENTRYPOINT ["dotnet", "LicenseApiProject.dll"]
