using EWS;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO.Converters;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory()));

builder.Services.AddHttpClient();

var connectionString = builder.Configuration.GetConnectionString("BohrungContext");
builder.Services.AddDbContext<EwsContext>(x => x.UseNpgsql(connectionString, option => option.UseNetTopologySuite()));

var app = builder.Build();

var ewsContextOptBuilder = new DbContextOptionsBuilder<EwsContext>().UseNpgsql(connectionString, option => option.UseNetTopologySuite());
using var context = new EwsContext(ewsContextOptBuilder.Options);
context.Database.Migrate();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Only seed if database is empty
    if (!context.Standorte.Any()) context.SeedData();

    // Use HTTPS redirection in development environment only due to unwanted redirects
    // with jwilder/nginx-proxy reverse proxy in production mode.
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
