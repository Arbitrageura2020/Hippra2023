using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hippra;
using Hippra.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Hippra.Models.SQL;
using Microsoft.AspNetCore.Identity;
using FTEmailService;
using Hippra.Areas.Identity;
using Hippra.Services.Email;
using Hippra.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Hippra.Components.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
// Add services to the container.


builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString), ServiceLifetime.Transient);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


//builder.Services.AddTransient<ApplicationDbContext>(p =>
//        p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
//        .CreateDbContext());


builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddTransient<IUserStore<AppUser>, UserStore<AppUser>>();
builder.Services.AddTransient<UserManager<AppUser>>();


builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, AdditionalUserClaimsPrincipalFactory>();
builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents();
builder.Services.AddAutoMapper(typeof(Startup));


builder.Services.AddRazorPages();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

var emailAccount = Configuration["FTEmailAccount"];
var emailCred = Configuration["FTEmailCred"];
builder.Services.AddTransient<FTEmailService.IEmailSender, EmailService>(
      (provider) =>
      {
          var eAccount = emailAccount;
          var eCred = emailCred;
          return new EmailService(eAccount, eCred, "Hippra Admin", "Hippra@outlook.com");
      });

builder.Services.AddTransient<ProfileService>();
// means run this service in background
builder.Services.AddTransient<HippraService>();
builder.Services.AddTransient<CommonService>();
builder.Services.AddTransient<AzureStorage>();
builder.Services.AddTransient<INotificationsService, NotificationsService>();
builder.Services.AddTransient<IFollowService, FollowService>();
builder.Services.AddTransient<IHistoryLogService, HistoryLogService>();
builder.Services.AddTransient<ICaseService, CaseService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<AppUser>>();
builder.Services.AddScoped<IEmailService, SendgridEmailService>(client =>
{
    var appKey = Configuration["SendgridAppKey"];
    return new SendgridEmailService(appKey);
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.Run();
