using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

			services.AddTransient(m => new FileUploadService(WebHostEnvironment));
			services.AddScoped<IClientInterface, ClientImplementation>();

			var server = Configuration["Database:Server"];
			var port = Configuration["Database:Port"];
			var user = Configuration["Database:UserName"];
			var password = Configuration["Database:Password"];
			var database = Configuration["Database:Name"];
			var conString = $"Host={server};Port={int.Parse(port)};" +
				$"Database={database};User Id={user};Password={password}";
			Console.WriteLine($"DbConnection string :- {conString}");
			var os = Environment.Is64BitOperatingSystem ? "64" : "32";
			Console.WriteLine($"PosMaster Running. OS:{os} BIT -{Environment.OSVersion.VersionString} {Environment.MachineName}");

			services.AddDbContext<DatabaseContext>(options =>
			options.UseNpgsql(conString));

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

			services.AddMvc();
			services.AddMemoryCache();
			services.AddControllers();
			services.AddDistributedMemoryCache();
			services.AddControllersWithViews();
		}

		public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
		{
			if (WebHostEnvironment.IsDevelopment())
				app.UseDeveloperExceptionPage();
			else
				app.UseExceptionHandler("/Home/Error");

			loggerFactory.AddFile("Logs/PosMaster-{Date}.txt");
			app.UseStaticFiles(new StaticFileOptions
			{
				OnPrepareResponse = ctx =>
				{
					const int durationInSeconds = 60 * 60 * 24;
					ctx.Context.Response.Headers[HeaderNames.CacheControl] =
						"public,max-age=" + durationInSeconds;
				}
			});
			app.UseAuthentication();
			app.UseRouting();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			DatabaseInit.Seed(app);
		}
	}
}
