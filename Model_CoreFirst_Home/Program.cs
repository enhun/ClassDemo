using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// �t�m��Ʈw�A��
builder.Services.AddDbContext<GuestBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ���Ҭ����t�m
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

// �����j�� HTTPS ���ɦV�H�K����
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();