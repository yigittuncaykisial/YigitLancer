using Entities.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Contracts;
using Services;
using Services.Contracts;
using Services.Currency;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<RepositoryContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("sqlconnection"),
        b => b.MigrationsAssembly("YigitLancer"));
});

// Repositoryler
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IPurchaseRequestRepository, PurchaseRequestRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();

// Servisler
builder.Services.AddScoped<IServiceManager, ServiceManager>(provider =>
{
    var repoManager = provider.GetRequiredService<IRepositoryManager>();
    var passwordHasher = provider.GetRequiredService<IPasswordHasher<User>>();
    return new ServiceManager(repoManager, passwordHasher);
});

builder.Services.AddMemoryCache();

var baseUrl = builder.Configuration["CollectApi:BaseUrl"];
if (string.IsNullOrWhiteSpace(baseUrl) || baseUrl.Contains("?"))
    throw new InvalidOperationException("CollectApi:BaseUrl must be a root URL without query, e.g. https://api.collectapi.com/");



builder.Services.AddHttpClient<ICurrencyService, CollectApiCurrencyService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CollectApi:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("authorization", builder.Configuration["CollectApi:ApiKey"]!);

    // BU SATIRI SİL ➜ client.DefaultRequestHeaders.Add("content-type", "application/json");

    // İstersen response formatı için Accept ekle:
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
;
builder.Services.AddScoped<IUserService, UserManager>(provider =>
{
    var userRepo = provider.GetRequiredService<IUserRepository>();
    var passwordHasher = provider.GetRequiredService<IPasswordHasher<User>>();
    return new UserManager(userRepo, passwordHasher);
});
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<IJobService, JobManager>();
builder.Services.AddScoped<IReviewService, ReviewManager>();
builder.Services.AddScoped<IPurchaseService, PurchaseManager>();
builder.Services.AddScoped<IChatService, ChatManager>();


builder.Services.AddScoped<IPasswordHasher<Entities.Models.User>, PasswordHasher<Entities.Models.User>>();


// 🔹 Authentication ve AccessDenied ayarı
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Index"; // Login sayfası
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Yetki yoksa yönlendirme
    });

// 🔹 Admin Policy ekle
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "True"));
    options.AddPolicy("FreelancerOnly", policy => policy.RequireClaim("UserJob", "Freelancer")); // <<—
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Authentication önce
app.UseAuthorization(); // Authorization sonra

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "Admin",
        areaName: "Admin",
        pattern: "Admin/{controller=Admin}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
