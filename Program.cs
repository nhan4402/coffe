using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shopping_Coffee.Areas.Admin.Repository;
using Shopping_Coffee.Models;
using Shopping_Coffee.Models.Momo;
using Shopping_Coffee.Repository;
using Shopping_Coffee.Services.Momo;

var builder = WebApplication.CreateBuilder(args);
//dbcontext
builder.Services.AddDbContext<DataContext>(Options =>
{
    Options.UseSqlServer(builder.Configuration["ConnectionStrings:DbConnection"]);
});

//Connect MomoAPI
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();
builder.Services.AddScoped<MomoService>();


//Add Email Sender
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});
//khai bao identity
builder.Services.AddIdentity<AppUserModel, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;

    // User settings.
    options.User.RequireUniqueEmail = false;
});

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



//Seeding data
//var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
//SeedData.SeedingData(context);

app.Run();
