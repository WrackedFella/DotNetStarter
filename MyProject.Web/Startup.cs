using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyProject.Domain;
using System;

namespace MyProject.Web
{
    public class Startup
    {
        private readonly string _connectionString;
        public Startup(IConfiguration configuration)
        {
            this._connectionString = configuration.GetConnectionString("Local");
#if TEST
            this._connectionString = configuration.GetValue<string>("Test");
#elif RELEASE
            this._connectionString = configuration.GetValue<string>("Release");
#endif

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void ConfigureContexts(IServiceCollection services)
        {
            services.AddDbContext<MyProjectContext>(options =>
            {
                options.UseSqlServer(this._connectionString);
#if DEBUG
                options.EnableSensitiveDataLogging();
#endif
            });

        }

        private void ConfigureAuth(IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = new TimeSpan(0, this.Configuration.GetValue<int>("AuthTimeoutMinutes"), 0);
                    options.SlidingExpiration = true;
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Logout";
                    options.AccessDeniedPath = "/Unauthorized";
                    options.ReturnUrlParameter = "returnUrl";
                });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            ConfigureLogging(services);
            services.AddSingleton(this.Configuration);
            ConfigureContexts(services);
            services.AddScoped<MyProjectContext>();

            services.AddRazorPages();
        }

        private void ConfigureLogging(IServiceCollection services)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()
                    .AddEventLog();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();
            services.AddSingleton(logger);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}