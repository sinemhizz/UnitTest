//Scaffold-DbContext
//"Data Source=DESKTOP-SVJNK7Q;Initial Catalog=UdemyUnitTestDb;Integrated Security=True;
//Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
//Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models


using Microsoft.EntityFrameworkCore;
using UnitTest.Web.Models;
using UnitTest.Web.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddDbContext<UdemyUnitTestDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["SqlConStr"]);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
