using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
			var clientId = Guid.NewGuid();
			var instanceId = Guid.NewGuid();
			var context = serviceProvider.GetService<DatabaseContext>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
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
					UserName = Constants.SuperAdminUserName,
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
					Personnel = Constants.SuperAdminUserName
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
					Code = "DEFAULT INST",
					Name = "DATABASE INSTANCE",
					Personnel = Constants.SuperAdminUserName
				};
				context.ClientInstances.Add(instance);
				context.SaveChanges();
			}

			if (!context.Customers.Any())
			{
				var customer = new Customer
				{
					Code = Constants.WalkInCustomerCode,
					FirstName = "WALK IN",
					Gender = "----",
					PhoneNumber = "0000000000",
					ClientId = clientId,
					InstanceId = instanceId,
					Personnel = Constants.SuperAdminUserName
				};
				context.Customers.Add(customer);
				context.SaveChanges();
			}

			if (!context.ExpenseTypes.Any())
			{
				var expenseType = new ExpenseType
				{
					ClientId = clientId,
					InstanceId = instanceId,
					Name = "DEFAULT TYPE",
					Code = "DEFAULT",
					Personnel = Constants.SuperAdminUserName
				};
				context.ExpenseTypes.Add(expenseType);
				context.SaveChanges();
			}

			if (!context.PaymentModes.Any())
			{
				var payment = new PaymentMode
				{
					ClientId = clientId,
					InstanceId = instanceId,
					Name = "CASH",
					Code = "CASH",
					Personnel = Constants.SuperAdminUserName
				};
				context.PaymentModes.Add(payment);
				context.SaveChanges();
			}

			if (!context.ProductCategories.Any())
			{
				var productCategory = new ProductCategory
				{
					ClientId = clientId,
					InstanceId = instanceId,
					Name = "DEFAULT",
					Code = "DEFAULT",
					Personnel = Constants.SuperAdminUserName
				};
				context.ProductCategories.Add(productCategory);
				context.SaveChanges();
			}

			if (!context.UnitOfMeasures.Any())
			{
				var unitOfMeasure = new UnitOfMeasure
				{
					ClientId = clientId,
					InstanceId = instanceId,
					Name = "PIECES",
					Code = "PIECES",
					Personnel = Constants.SuperAdminUserName
				};
				context.UnitOfMeasures.Add(unitOfMeasure);
				context.SaveChanges();
			}
		}
	}
}
