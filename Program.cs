using LokerMVC;
using LokerMVC.Filters.ActionFilters;
using MyIdentityServer.Mvc;
using MyMiddleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllersWithViews(options =>
    {
        options.Filters.Add<UrlActionFilter>();
    })
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddAppSettingService(builder.Configuration);
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromSeconds(60); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

var siteSetting = builder.Configuration.GetValue<string>("ApiUrlInternal:UrlMySite");
var site = new Uri(siteSetting).Host;
app.UseNonWww(site);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseSession();

app.UseSetClaimClient();
app.UseAntiforgery();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");





app.Run();