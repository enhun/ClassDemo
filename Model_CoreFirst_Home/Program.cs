using Microsoft.EntityFrameworkCore;
using Model_CoreFirst_Home.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();


//���USession�A��
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option=>
{
    option.IdleTimeout = TimeSpan.FromMinutes(10);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});


builder.Services.AddDbContext<GuestBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddLogging();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
        Console.WriteLine("��ƪ�l�Ʀ��\�����C");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"��ƪ�l�ƹL�{�o�Ϳ��~: {ex.Message}");
    }
}

//if (!app.Environment.IsDevelopment())
//{
    app.UseExceptionHandler("/Home/Error");
app.UseStatusCodePagesWithReExecute("/Home/Error");
//}
//else
//{
 //   app.UseDeveloperExceptionPage();
//}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();


//�ҥ�Session
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();