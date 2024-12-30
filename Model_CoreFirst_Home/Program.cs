using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 配置資料庫服務
builder.Services.AddDbContext<GuestBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 環境相關配置
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

// 移除強制 HTTPS 重導向以便測試
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();