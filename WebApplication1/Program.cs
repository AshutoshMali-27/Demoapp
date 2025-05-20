
using Microsoft.EntityFrameworkCore;
using Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.DotNet.Scaffolding.Shared;
using Web.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;


var builder = WebApplication.CreateBuilder(args);
// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "RequestVerificationToken";
    options.Cookie.HttpOnly = true;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IFileSystem, DefaultFileSystem>();
var apiSettingsSection = builder.Configuration.GetSection("ApiSettings");
var httpClientSettingsSection = builder.Configuration.GetSection("HttpClientSettings");
var httpClientTimeout = httpClientSettingsSection.GetValue<TimeSpan>("Timeout", TimeSpan.FromMinutes(3));
builder.Services.AddHttpClient("HttpClientForApiTimeOut", client =>
{
    client.Timeout = httpClientTimeout;
});
var app = builder.Build();

ApiLoggerExtensions.Initialize(app.Environment);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
app.UseHttpsRedirection();
//app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (!app.Environment.IsDevelopment() && (ctx.Context.Request.Path.StartsWithSegments("/Script/Pages") ||
                ctx.Context.Request.Path.StartsWithSegments("/Script/init")))
        {
            //ctx.Context.Response.Headers["Cache-Control"] = "public,max-age=31536000"; // 1 year
            ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            ctx.Context.Response.Headers["Pragma"] = "no-cache";
            ctx.Context.Response.Headers["Expires"] = "-1";
        }
        else if (app.Environment.IsDevelopment() && (ctx.Context.Request.Path.StartsWithSegments("/Script/Pages") ||
                ctx.Context.Request.Path.StartsWithSegments("/Script/init")))
        {
            ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            ctx.Context.Response.Headers["Pragma"] = "no-cache";
            ctx.Context.Response.Headers["Expires"] = "-1";
        }
    }
});
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");
app.Run();
