using Microsoft.EntityFrameworkCore;
using Single_Page_Application.HostedService;
using Single_Page_Application.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HUSDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("db")));
builder.Services.AddHostedService<DbSeederHostedService>();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
var app = builder.Build();

app.UseStaticFiles();
app.MapControllerRoute(
    name:"default",
    pattern:"{controller=Main}/{action=Index}/{id?}"
    );


app.Run();
