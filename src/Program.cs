using EWS;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("BohrungContext");
builder.Services.AddDbContext<EwsContext>(x => x.UseNpgsql(connectionString, option => option.UseNetTopologySuite()));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory()));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    var ewsContextOptBuilder = new DbContextOptionsBuilder<EwsContext>().UseNpgsql(connectionString, option => option.UseNetTopologySuite());
    using var context = new EwsContext(ewsContextOptBuilder.Options);

    // Only seed if database is empty
    if (!context.Standorte.Any()) context.SeedData();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
