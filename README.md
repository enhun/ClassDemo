# .NET Core MVC 留言板專案

這是一個使用 .NET Core MVC 建立的留言板系統，實現了基本的貼文和回覆功能。

## 功能特點

- 貼文的新增、編輯、刪除功能
- 回覆留言功能
- 圖片上傳功能
- 種子資料自動初始化

## 系統需求
.NET Core SDK 7.0 或更新版本
Visual Studio 2022 或 VS Code
SQL Server (LocalDB 或完整版本)


## 安裝步驟

Clone 專案到本地:

git clone [您的 Repository URL]

### 確保 appsettings.json 中的資料庫連接字串正確:


{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}


### 開啟命令提示字元，切換到專案資料夾，執行以下命令更新資料庫:


dotnet ef database update


### 建立必要的資料夾:

mkdir wwwroot/bookphotos


### 初始化資料
SeedData.cs 包含初始資料，確保在 Program.cs 中有以下程式碼:

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<GuestBookContext>();
        SeedData.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

### 圖片處理

確保 wwwroot/bookphotos 資料夾具有寫入權限
上傳的圖片會自動存放在此資料夾中
如果從其他電腦複製專案，需要手動複製 wwwroot/bookphotos 中的圖片

### 常見問題解決

### 資料庫連接問題：

確認 SQL Server 是否正確安裝
檢查連接字串是否正確
確保資料庫使用者具有適當權限


### 圖片無法顯示：

檢查 wwwroot/bookphotos 資料夾是否存在
確認圖片檔案是否已正確複製
檢查資料夾權限設定


### Seed Data 問題：

確保資料庫已正確更新
檢查 Program.cs 中的初始化程式碼
查看應用程式記錄檔以瞭解可能的錯誤



## 注意事項

### 資料庫遷移：

專案包含 Migrations 資料夾，請勿刪除
首次執行時必須執行資料庫更新命令


### 圖片管理：

建議定期備份 wwwroot/bookphotos 資料夾
注意圖片檔案大小限制


### 環境設定：

開發環境與生產環境的 appsettings.json 可能需要不同設定
請根據實際需求調整資料庫連接字串



##  部署建議

發布前檢查清單：

更新所有 NuGet 套件
確認資料庫連接字串
測試所有功能
備份重要資料


## IIS 部署注意事項：

設定適當的應用程式池
確保 IIS 用戶具有足夠權限
設定正確的檔案權限



## 其他注意事項


### 對於 SeedData.cs，建議修改初始化方法，確保資料只會被植入一次：



public static void Initialize(GuestBookContext context)
{
   
    if (context.Books.Any()) // 檢查是否已有資料
    {
       
        return; // 如果已有資料，直接返回
    
    }
    
    // 添加初始資料...
}

### 建議在 Program.cs 中添加圖片目錄的自動創建：



if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bookphotos")))
{
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bookphotos"));
}
