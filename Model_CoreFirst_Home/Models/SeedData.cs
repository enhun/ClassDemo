

using Microsoft.EntityFrameworkCore;

namespace Model_CoreFirst_Home.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new GuestBookContext(
                serviceProvider.GetRequiredService<DbContextOptions<GuestBookContext>>()))
            {
                // 檢查是否已有資料
                if (!context.Book.Any())
                {
                    // 建立 GUID 作為唯一識別碼
                    string[] MyGuid = {
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString()
                    };

                    // 新增主要貼文資料
                    context.Book.AddRange(
                        new Book
                        {
                            BookID = MyGuid[0],
                            Title = "櫻桃鴨",
                            Description = "這看起來好好吃哦!!!",
                            Author = "Jack",
                            Photo = MyGuid[0] + ".jpg",
                            ImageType = "image/jpeg",
                            TimeStamp = DateTime.Now
                        },
                        new Book
                        {
                            BookID = MyGuid[1],
                            Title = "鴨油高麗菜",
                            Description = "好像稍微有點油....",
                            Author = "Mary",
                            Photo = MyGuid[1] + ".jpg",
                            ImageType = "image/jpeg",
                            TimeStamp = DateTime.Now
                        },
                                 new Book
                                 {
                                     BookID = MyGuid[2],
                                     Title = "鴨油麻婆豆腐",
                                     Description = "這太下飯了！可以吃好幾碗白飯",
                                     Photo = MyGuid[2] + ".jpg",
                                     ImageType = "image/jpeg",
                                     Author = "王小花",
                                     TimeStamp = DateTime.Now
                                 },
                                 new Book
                                 {
                                     BookID = MyGuid[3],
                                     Title = "櫻桃鴨握壽司",
                                     Description = "握壽司就是好吃！",
                                     Photo = MyGuid[3] + ".jpg",
                                     ImageType = "image/jpeg",
                                     Author = "王小花",
                                     TimeStamp = DateTime.Now
                                 },
                                 new Book
                                 {
                                     BookID = MyGuid[4],
                                     Title = "三杯鴨",
                                     Description = "鴨肉鮮甜",
                                     Photo = MyGuid[4] + ".jpg",
                                     ImageType = "image/jpeg",
                                     Author = "Jack",
                                     TimeStamp = DateTime.Now
                                 }
                    // ... 其他貼文資料 ...
                    );

                    context.SaveChanges();

                    // 新增回覆資料
                    string[] MyGuidRe = {
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString(),
                         Guid.NewGuid().ToString(), 
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString() 
                        // ... 更多 GUID ...
                    };

                    context.ReBook.AddRange(
                        new ReBook
                        {
                            ReBookID = MyGuidRe[0],
                            Description = "我也覺得好吃！",
                            Author = "小蘭",
                            TimeStamp = DateTime.Now,
                            BookID = MyGuid[0]
                        },
                        new ReBook
                        {
                            ReBookID = MyGuidRe[1],
                            Description = "我不喜歡....",
                            Author = "柯南",
                            TimeStamp = DateTime.Now,
                            BookID = MyGuid[0]
                        },
                        new ReBook
                        {
                            ReBookID = MyGuidRe[2],
                            Description = "你最好餓死",
                            Author = "小蘭",
                            TimeStamp = DateTime.Now,
                            BookID = MyGuid[0]
                        },
                        new ReBook
                        {
                            ReBookID = MyGuidRe[3],
                            Description = "高麗菜這樣超好吃啊～",
                            Author = "小英",
                            TimeStamp = DateTime.Now,
                            BookID = MyGuid[1]
                        },
                            new ReBook
                            {
                                ReBookID = MyGuidRe[4],
                                Description = "口味似乎偏辣",
                                Author = "阿狗",
                                TimeStamp = DateTime.Now,
                                BookID = MyGuid[2]
                            },
                            new ReBook
                            {
                                ReBookID = MyGuidRe[5],
                                Description = "我還是喜歡生魚片的握壽司",
                                Author = "嫩嫩",
                                TimeStamp = DateTime.Now,
                                BookID = MyGuid[3]
                            },
                            new ReBook
                            {
                                ReBookID = MyGuidRe[6],
                                Description = "我也是喜歡生魚片的握壽司，但這個也不錯",
                                Author = "王小花",
                                TimeStamp = DateTime.Now,
                                BookID = MyGuid[3]
                            },
                            new ReBook
                            {
                                ReBookID = MyGuidRe[7],
                                Description = "三杯雞比較對味",
                                Author = "芷若",
                                TimeStamp = DateTime.Now,
                                BookID = MyGuid[4]
                            }
                    // ... 其他回覆資料 ...
                    );

                    context.SaveChanges();
                    // 複製種子照片
                    try
                    {
                        string seedPhotosPath = Path.Combine(Directory.GetCurrentDirectory(), "SeedPhotos");
                        string bookPhotosPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BookPhotos");

                        // 確認來源資料夾存在
                        if (!Directory.Exists(seedPhotosPath))
                        {
                            throw new DirectoryNotFoundException($"找不到種子照片資料夾: {seedPhotosPath}");
                        }

                        // 確認目標資料夾存在，若不存在就建立
                        if (!Directory.Exists(bookPhotosPath))
                        {
                            Directory.CreateDirectory(bookPhotosPath);
                        }

                        var files = Directory.GetFiles(seedPhotosPath);
                        Console.WriteLine($"找到 {files.Length} 個檔案準備複製");

                        for (int i = 0; i < files.Length; i++)
                        {
                            string destFile = Path.Combine(bookPhotosPath, MyGuid[i] + ".jpg");
                            File.Copy(files[i], destFile, true);  // 加入 true 參數允許覆蓋現有檔案
                            Console.WriteLine($"已複製: {files[i]} => {destFile}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"複製照片時發生錯誤: {ex.Message}");
                        // 可以選擇將錯誤訊息記錄到日誌檔案
                    }
                }
            }
        }
    }
}
