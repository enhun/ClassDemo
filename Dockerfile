# 使用官方 .NET SDK 映像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 複製解決方案和專案檔案
COPY Model_CoreFirst_Home.sln .
COPY Model_CoreFirst_Home/Model_CoreFirst_Home.csproj ./Model_CoreFirst_Home/
RUN dotnet restore

# 複製所有專案檔案
COPY . .
WORKDIR /app/Model_CoreFirst_Home
RUN dotnet publish -c Release -o /app/out

# 創建運行時映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# 設定容器啟動時執行的命令
ENTRYPOINT ["dotnet", "Model_CoreFirst_Home.dll"]