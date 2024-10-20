using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Hippra.Areas.Identity;
using Hippra.Data;
using FTEmailService;
using Hippra.Models.SQL;
using Hippra.Services;
using AutoMapper;
using System.Runtime.InteropServices;
using Hippra.Services.Email;

namespace Hippra
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

                services.AddDbContextFactory<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });
                

            services.AddTransient<ApplicationDbContext>(p => 
                    p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
                    .CreateDbContext());

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, AdditionalUserClaimsPrincipalFactory>();

            services.AddAutoMapper(typeof(Startup));
            

            services.AddRazorPages();
            //services.AddMvc();
            services.AddRazorComponents()
          .AddInteractiveServerComponents();
            services.AddHttpContextAccessor();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            string emailAccount;
            string emailCred;

            if (CurrentEnvironment.IsDevelopment())
            {
                //emailAccount = Configuration["SecAppSettings:FTEmailAccount"];
                //emailCred = Configuration["SecAppSettings:FTEmailCred"];
                //Configuration.GetSection("AppSettings").GetSection("FTEmailAccount").Value = emailAccount;
                //Configuration.GetSection("AppSettings").GetSection("FTEmailCred").Value = emailCred;
                emailAccount = Configuration.GetSection("AppSettings").GetSection("FTEmailAccount").Value;
                emailCred = Configuration.GetSection("AppSettings").GetSection("FTEmailCred").Value;
                services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            }
            else
            {
                emailAccount = Configuration["FTEmailAccount"];
                emailCred = Configuration["FTEmailCred"];
                // can't believe this worked! basically override the built configuration with value from azure, then everything is the same
                Configuration.GetSection("AppSettings").GetSection("StorageConnectionString").Value = Configuration["StorageConnectionString"];
                Configuration.GetSection("AppSettings").GetSection("StorageRootContainer").Value = Configuration["StorageRootContainer"];

                Configuration.GetSection("AppSettings").GetSection("FTEmailAccount").Value = Configuration["FTEmailAccount"];
                Configuration.GetSection("AppSettings").GetSection("FTEmailCred").Value = Configuration["FTEmailCred"];
                Configuration.GetSection("AppSettings").GetSection("SupervisorUsr").Value = Configuration["SupervisorUsr"];
                Configuration.GetSection("AppSettings").GetSection("FTManagerUsr").Value = Configuration["FTManagerUsr"];
                Configuration.GetSection("AppSettings").GetSection("FTManagerPwd").Value = Configuration["FTManagerPwd"];
                Configuration.GetSection("AppSettings").GetSection("RootUrl").Value = Configuration["RootUrl"];
                Configuration.GetSection("AppSettings").GetSection("StorageUrl").Value = Configuration["StorageUrl"];
                services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            }
            //AddSingleton AddScoped AddTransient
            //services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<FTEmailService.IEmailSender, EmailService>(
                (provider) =>
                {
                    var eAccount = emailAccount;
                    var eCred = emailCred;
                    return new EmailService(eAccount, eCred, "Hippra Admin", "Hippra@outlook.com");
                });

            services.AddTransient<ProfileService>();
            // means run this service in background
            services.AddTransient<HippraService>();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<AppUser>>();
            services.AddScoped<IEmailService, SendgridEmailService>(client =>
            {
                var appKey = Configuration["SendgridAppKey"];
                return new SendgridEmailService(appKey);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapRazorComponents<App>()
              .AddInteractiveServerRenderMode();
            });
          
        }
    }
}
