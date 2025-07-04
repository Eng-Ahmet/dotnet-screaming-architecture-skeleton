# مرحلة البناء
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# نسخ الملفات
COPY . ./

# استرجاع الحزم
RUN dotnet restore

# بناء ونشر المشروع
RUN dotnet publish -c Release -o out

# مرحلة التشغيل
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# نسخ الملفات من مرحلة البناء
COPY --from=build /app/out .

# فتح المنفذ 5000
EXPOSE 5000

# بدء التشغيل
ENTRYPOINT ["dotnet", "api.dll"]
