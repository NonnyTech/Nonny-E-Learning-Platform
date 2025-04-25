using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.AppSetting;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Helper;
using NonnyE_Learning.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("MyConnections");


builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connectionString));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    var allowed = options.User.AllowedUserNameCharacters
      + "/-@.";
    options.User.AllowedUserNameCharacters = allowed;
    options.Password.RequiredLength = 3;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(3);
    options.Cookie.IsEssential = true;

});
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(150);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = new PathString("/Home/AccessDenied");
    options.SlidingExpiration = true;
});
builder.Services.Configure<FlutterwaveConfig>(builder.Configuration.GetSection("FlutterwaveConfig"));

builder.Services.AddTransient<IAuthServices, AuthServices>();
builder.Services.AddTransient<ICourseServices, CourseService>();
builder.Services.AddTransient<IEmailServices, EmailServices>();
builder.Services.AddTransient<ITransactionServices, TransactionServices>();
builder.Services.AddTransient<IEnrollmentServices, EnrollmentServices>();
builder.Services.AddTransient<IModuleServices, ModuleServices>();
builder.Services.AddTransient<ICertificateService, CertificateServices>();
builder.Services.AddHttpClient<IFlutterwaveServices, FlutterwaveServices>();



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

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
	var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
	await SeedDataHelper.SeedAdminDataAsync(services, userManager, roleManager);

}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
