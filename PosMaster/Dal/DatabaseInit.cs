﻿using MailKit.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PosMaster.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal
{
	public static class DatabaseInit
	{
		public static async void Seed(IApplicationBuilder app)
		{
			using var serviceScope = app.ApplicationServices.CreateScope();
			await SeedDataAsync(serviceScope.ServiceProvider);
		}

		private static async Task SeedDataAsync(IServiceProvider serviceProvider)
		{
			var clientId = Guid.Parse("bb0a26bc-1e0a-4b60-a5a4-7ffb43c060c6");
			var instanceId = Guid.Parse("f1781e2d-577d-4db6-9565-e7718b86cc7f");
			var context = serviceProvider.GetService<DatabaseContext>();
			Console.WriteLine("Applying migrations ...");
			context.Database.Migrate();
			Console.WriteLine("Seeding default data ...");
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
			var clientInterface = serviceProvider.GetRequiredService<IClientInterface>();
			var roleNames = new List<string> {
				Role.SuperAdmin, Role.Admin, Role.Clerk, Role.Manager
			};

			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
					await roleManager.CreateAsync(new IdentityRole(roleName));
			}
			var userEmail = Constants.SuperAdminEmail;
			var user = await userManager.FindByEmailAsync(userEmail);
			if (user == null)
			{
				var poweruser = new User
				{
					UserName = Constants.SuperAdminEmail,
					Email = Constants.SuperAdminEmail,
					FirstName = "SYS",
					LastName = "ADMIN",
					PhoneNumber = "0733208119",
					Role = Role.SuperAdmin,
					Gender = "Male",
					EmailConfirmed = true,
					Status = EntityStatus.Active,
					ClientId = clientId,
					InstanceId = instanceId,
					IdNumber = "----"
				};

				var realPwd = Constants.SuperAdminPassword;
				var createPowerUser = await userManager.CreateAsync(poweruser, realPwd);
				if (createPowerUser.Succeeded)
				{
					await userManager.AddToRoleAsync(poweruser, Role.Manager);
					await userManager.AddToRoleAsync(poweruser, Role.Admin);
					await userManager.AddToRoleAsync(poweruser, Role.SuperAdmin);
				}
			}

			if (!context.SystemSettings.Any())
			{
				var settings = new SystemSetting
				{
					ClientId = clientId,
					InstanceId = instanceId,
					Personnel = Constants.SuperAdminEmail,
					Name = "PosMaster",
					Version = "1.0.1",
					Tagline = "Best POS in the Market",
					TermsAndConditions = "USE AS IS",
					EmailAddress = Constants.SystemEmailAddress
				};
				context.SystemSettings.Add(settings);
				context.SaveChanges();
			}

			if (!context.Clients.Any())
			{
				var client = new Client
				{
					Id = clientId,
					ClientId = clientId,
					InstanceId = instanceId,
					Name = "DEFAULT CLIENT",
					CountryFull = "KENYA",
					CountryShort = "KE",
					CurrencyFull = "KENYA SHILLINGS",
					CurrencyShort = "KES",
					Code = "DEFAULT",
					EnforcePassword = true,
					PasswordExpiryMonths = 1,
					Town = "NAIROBI",
					PhoneNumberLength = 10,
					TelephoneCode = "+254",
					DisplayBuyingPrice = true,
					Personnel = Constants.SuperAdminEmail
				};
				context.Clients.Add(client);
				context.SaveChanges();
			}

			if (!context.ClientInstances.Any())
			{
				var instance = new ClientInstance
				{
					Id = instanceId,
					InstanceId = instanceId,
					ClientId = clientId,
					Code = "DEFAULT",
					Name = "DEFAULT INSTANCE",
					Personnel = Constants.SuperAdminEmail
				};
				context.ClientInstances.Add(instance);
				context.SaveChanges();
			}

			clientInterface.SeedDefaultData(clientId, instanceId, true);
			Console.WriteLine("Done. Seeding complete");
		}
	}
}
