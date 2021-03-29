using MailKit.Security;
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
            var clientId = Guid.Parse("2296F559-2432-496B-96EC-C6324D3A217D");
            var instanceId = Guid.Parse("A5703309-AF3B-48F9-BD3F-8D2FCAB8312D");
            var context = serviceProvider.GetService<DatabaseContext>();
            Console.WriteLine("Applying migrations ...");
            context.Database.Migrate();
            Console.WriteLine("Seeding default data ...");

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
                    await userManager.AddToRoleAsync(poweruser, Role.SuperAdmin);
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
            clientInterface.SeedDefaultData(clientId, instanceId, true);
            Console.WriteLine("Done. Seeding complete");
        }
    }
}
