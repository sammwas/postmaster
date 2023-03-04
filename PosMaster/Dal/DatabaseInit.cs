using MailKit.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PosMaster.Dal.Interfaces;
using PosMaster.Services;
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
            Console.WriteLine("Applying migrations ...");
            context.Database.Migrate();
            Console.WriteLine("Seeding default data ...");

            if (!context.Clients.Any())
            {
                Console.WriteLine($"No default client - add client {clientId}");
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
                    MaxInstance = 2,
                    PasswordExpiryMonths = 1,
                    Town = "NAIROBI",
                    PhoneNumberLength = 10,
                    TelephoneCode = "+254",
                    DisplayBuyingPrice = true,
                    Personnel = Constants.SuperAdminEmail
                };
                client.Licence = LicencingService.GenerateLicence(clientId, 1, 3745, client.Name).Data.Licence;
                context.Clients.Add(client);
                context.SaveChanges();

                var instance = new ClientInstance
                {
                    Id = instanceId,
                    InstanceId = instanceId,
                    ClientId = clientId,
                    Code = "DEFAULT",
                    Name = "DEFAULT INSTANCE",
                    ReceiptFontPercent = 100,
                    Personnel = Constants.SuperAdminEmail
                };
                context.ClientInstances.Add(instance);
                context.SaveChanges();

                var emailSetting = new EmailSetting
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Personnel = Constants.SuperAdminEmail,
                    SenderFromEmail = "support@qilimo.co.ke",
                    SmtpServer = "mail.qilimo.co.ke",
                    SmtpPort = 587,
                    SocketOptions = SecureSocketOptions.StartTls,
                    SmtpPassword = Constants.SuperAdminPassword
                };
                context.EmailSettings.Add(emailSetting);
                context.SaveChanges();

                var systemSetting = new SystemSetting
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
                context.SystemSettings.Add(systemSetting);
                context.SaveChanges();

                var clientInterface = serviceProvider.GetRequiredService<IClientInterface>();
                clientInterface.SeedDefaultData(clientId, instanceId);
            }

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

            var user = await userManager.FindByEmailAsync(Constants.SuperAdminEmail);
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
            Console.WriteLine("Done. Seeding complete");
        }
    }
}
