using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using PosMaster.Dal;
using PosMaster.Dal.Interfaces;
using PosMaster.Services;
using PosMaster.Services.Messaging;
using System;

namespace PosMaster
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment WebHostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient(m => new FileUploadService(WebHostEnvironment));
            services.AddScoped<ICookiesService, CookiesService>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserInterface, UserInterface>();
            services.AddScoped<IClientInterface, ClientImplementation>();
            services.AddScoped<IClientInstanceInterface, ClientInstanceImplementation>();
            services.AddScoped<ISystemSettingInterface, SystemSettingImplementation>();
            services.AddScoped<IProductInterface, ProductImplementation>();
            services.AddScoped<IExpenseInterface, ExpenseImplementation>();
            services.AddScoped<ICustomerInterface, CustomerImplementation>();
            services.AddScoped<ISupplierInterface, SupplierImplementation>();
            services.AddScoped<IDashboardInterface, DashboardImplementation>();
            services.AddScoped<IReportingInterface, ReportingImplementation>();
            services.AddScoped<IMasterDataInterface, MasterDataImplementation>();
            services.AddScoped<IInvoiceInterface, InvoiceImplementation>();
            services.AddScoped<IOrderInterface, OrdersImplementation>();
            services.AddScoped<IHumanResourceInterface, HumanResourceImplementation>();

            var server = Configuration["Database:Server"] ?? "localhost";
            var port = Configuration["Database:Port"] ?? "5432";
            var user = Configuration["Database:UserName"] ?? "postgres";
            var password = Configuration["Database:Password"] ?? "123456";
            var database = Configuration["Database:Name"] ?? "posmater_db";
            var from = "Docker-ENV";
            if (!string.IsNullOrEmpty(Configuration["Database:Name"]))
            {
                from = "Config-File";
                password = EncypterService.Decrypt(password);
                user = EncypterService.Decrypt(user);
            }
            var conString = "";
            var os = Environment.Is64BitOperatingSystem ? "64" : "32";
            Console.WriteLine($"PosMaster Running. OS:{os} BIT -{Environment.OSVersion.VersionString} {Environment.MachineName}");

            if (WebHostEnvironment.IsDevelopment())
                conString = $"Host={server};Port={int.Parse(port)};" +
                $"Database={database};User Id={user};Password={password}";
            else
            {
                // Use connection string provided at runtime by FlyIO.
                var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                // Parse connection URL to connection string for Npgsql
                connUrl = connUrl.Replace("postgres://", string.Empty);
                var pgUserPass = connUrl.Split("@")[0];
                var pgHostPortDb = connUrl.Split("@")[1];
                var pgHostPort = pgHostPortDb.Split("/")[0];
                var pgDb = pgHostPortDb.Split("/")[1];
                var pgUser = pgUserPass.Split(":")[0];
                var pgPass = pgUserPass.Split(":")[1];
                var pgHost = pgHostPort.Split(":")[0];
                var pgPort = pgHostPort.Split(":")[1];
                var updatedHost = pgHost.Replace("flycast", "internal");

                conString = $"Server={updatedHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
            }

            services.AddDbContext<DatabaseContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                Console.WriteLine($"{env} DbConnection String > Src: {from} Server: {server} Port: {port} Database: {database} User: {user}");
                options.UseNpgsql(conString)
                 .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            });

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.@";
                options.User.RequireUniqueEmail = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

                options.LoginPath = "/Home/Index";
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.SlidingExpiration = true;
            });

            //services.AddAuthentication()
            //    .AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = Configuration["Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["Google:ClientSecret"];
            //});

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                //options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddMvc();
            services.AddMemoryCache();
            services.AddControllers();
            services.AddDistributedMemoryCache();
            services.AddControllersWithViews();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            if (WebHostEnvironment.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            var path = Configuration["LogsPath"] ?? "Logs";
            //loggerFactory.AddFile($"{path}/PosMaster-{DateTime.Now.Date}.log");
            loggerFactory.AddFile(path + "/PosMaster-{Date}.log");
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;
                }
            });

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<InAppChatHub>($"/chatHub");
            });

            DatabaseInit.Seed(app);
        }
    }
}
