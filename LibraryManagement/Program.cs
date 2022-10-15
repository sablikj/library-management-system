using LibraryManagement.Models;
using LibraryManagement.Models.Identity;
using LibraryManagement.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var dbSettings = builder.Configuration.GetSection("LibraryManagementDatabase").Get<DatabaseSettings>();

// Db init ?
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("LibraryManagementDatabase"));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
    dbSettings.ConnectionString, dbSettings.Name
    );

builder.Services.AddSingleton<BooksService>();

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

app.UseAuthentication();
app.UseAuthorization();

/*
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
*/

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
