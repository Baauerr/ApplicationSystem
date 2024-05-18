using AdminPanel.BL.Serivces.Impl;
using AdminPanel.BL.Serivces.Interface;
using Common.Helpers;
using Common.Helpers.Impl;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IQueueSender, QueueSender>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenHelper, TokenHelper>();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
