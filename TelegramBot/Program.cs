using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Entities;
using TelegramBot.Models.Manager;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DatabaseContext>(opts =>
{                 
    opts.UseNpgsql(builder.Configuration.GetConnectionString("MyPostgresConnection"));
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opts =>
    {
        opts.Cookie.Name = ".TelegramBot.auth";
        opts.ExpireTimeSpan = TimeSpan.FromDays(7);    //tekrar bak
        opts.SlidingExpiration = false;
        opts.LoginPath = "/Account/Login";
        opts.LogoutPath = "/Account/Logout";
        opts.AccessDeniedPath = "/Home/AccesDenied";     //buraya tekrar bak
    });

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient("7423136173:AAE6-79KCB8rHjOauqSk5wyOj9y_soYooqI"));
builder.Services.AddScoped<TelegramServices>();
builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger"; // Swagger UI'yi /swagger altýnda eriþilebilir hale getirir
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
