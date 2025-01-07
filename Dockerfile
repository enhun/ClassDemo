# 使用官方 .NET SDK 映像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 複製 .sln 和 .csproj 檔案
COPY *.sln .
COPY *.csproj .
RUN dotnet restore

# 複製所有專案檔案
COPY . .
RUN dotnet publish -c Release -o out

# 創建運行時映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# 設定容器啟動時執行的命令
ENTRYPOINT ["dotnet", "Model_CoreFirst_Home.dll"]