using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.Services;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
    public interface IClientInterface
    {
        Task<ReturnData<Client>> EditAsync(ClientViewModel model);
        Task<ReturnData<List<Client>>> AllAsync();
        Task<ReturnData<Client>> ByIdAsync(Guid id);
        void SeedDefaultData(Guid clientId, Guid instanceId);
        Task<ReturnData<EmailSetting>> UpdateEmailSettingAsync(EmailSettingViewModel model);
        Task<ReturnData<EmailSetting>> ClientEmailSettingAsync(Guid clientId);
        ReturnData<string> UpdateLicence(Guid clientId, string licence, string personnel);
    }


    public class ClientImplementation : IClientInterface
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<ClientImplementation> _logger;
        public ClientImplementation(DatabaseContext context, ILogger<ClientImplementation> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ReturnData<string> UpdateLicence(Guid clientId, string licence, string personnel)
        {
            var res = new ReturnData<string>();
            try
            {
                var client = _context.Clients.FirstOrDefault(c => c.Id.Equals(clientId));
                if (client == null)
                {
                    res.Message = "Setting not Found";
                    return res;
                }

                var licStatus = LicencingService.VerifyLicence(licence, clientId, client.Name);
                if (!licStatus.Success)
                {
                    res.Message = licStatus.Message;
                    return res;
                }

                client.DateLastModified = DateTime.Now;
                client.Licence = licence;
                client.LastModifiedBy = personnel;
                _context.SaveChanges();

                res.Success = true;
                res.Message = "Licence Updated";
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                res.Message = "Error Occured. Try again";
                res.ErrorMessage = e.Message;
                return res;
            }
        }


        public async Task<ReturnData<List<Client>>> AllAsync()
        {
            var result = new ReturnData<List<Client>> { Data = new List<Client>() };
            var tag = nameof(AllAsync);
            _logger.LogInformation($"{tag} get all clients");
            try
            {
                var data = await _context.Clients
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} clients");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }

        }

        public async Task<ReturnData<Client>> ByIdAsync(Guid id)
        {
            var result = new ReturnData<Client> { Data = new Client() };
            var tag = nameof(ByIdAsync);
            _logger.LogInformation($"{tag} get client by id - {id}");
            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id.Equals(id));
                result.Success = client != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = client;
                _logger.LogInformation($"{tag} client {id} {result.Message}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public async Task<ReturnData<EmailSetting>> ClientEmailSettingAsync(Guid clientId)
        {
            var result = new ReturnData<EmailSetting> { Data = new EmailSetting() };
            var tag = nameof(ClientEmailSettingAsync);
            _logger.LogInformation($"{tag} get client email settings - {clientId}");
            try
            {
                var setting = await _context.EmailSettings
                    .FirstOrDefaultAsync(c => c.ClientId.Equals(clientId));
                result.Success = setting != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = setting;
                _logger.LogInformation($"{tag} email settings {clientId} {result.Message}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public async Task<ReturnData<Client>> EditAsync(ClientViewModel model)
        {
            var result = new ReturnData<Client> { Data = new Client() };
            var tag = nameof(EditAsync);
            _logger.LogInformation($"{tag} edit client");
            try
            {
                if (model.IsEditMode)
                {
                    var dbClient = await _context.Clients.FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbClient == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbClient.Code = model.Code;
                    dbClient.Name = model.Name;
                    dbClient.Slogan = model.Slogan;
                    dbClient.CurrencyFull = model.CurrencyFull;
                    dbClient.CurrencyShort = model.CurrencyShort;
                    dbClient.CountryFull = model.CountryFull;
                    dbClient.CountryShort = model.CountryShort;
                    dbClient.Vision = model.Vision;
                    dbClient.Mission = model.Mission;
                    dbClient.PasswordExpiryMonths = model.PasswordExpiryMonths;
                    dbClient.PostalAddress = model.PostalAddress;
                    dbClient.Town = model.Town;
                    dbClient.EmailAddress = model.EmailAddress;
                    dbClient.PrimaryColor = model.PrimaryColor;
                    dbClient.SecondaryColor = model.SecondaryColor;
                    dbClient.PrimaryTelephone = model.PrimaryTelephone;
                    dbClient.SecondaryTelephone = model.SecondaryTelephone;
                    dbClient.PhoneNumberLength = model.PhoneNumberLength;
                    dbClient.TelephoneCode = model.TelephoneCode;
                    dbClient.DisplayBuyingPrice = model.DisplayBuyingPrice;
                    dbClient.Notes = model.Notes;
                    dbClient.Status = model.Status;
                    dbClient.DateLastModified = DateTime.UtcNow;
                    dbClient.LastModifiedBy = model.Personnel;
                    if (model.IsNewImage)
                        dbClient.LogoPath = model.LogoPath;
                    await _context.SaveChangesAsync();
                    result.Data = dbClient;
                    result.Success = true;
                    result.Message = "Updated";
                    _logger.LogInformation($"{tag} updated {dbClient.Name} {model.Id} : {result.Message}");
                    return result;
                }
                var client = new Client
                {
                    Id = model.Id,
                    Code = model.Code,
                    Name = model.Name,
                    Slogan = model.Slogan,
                    CurrencyFull = model.CurrencyFull,
                    CurrencyShort = model.CurrencyShort,
                    Vision = model.Vision,
                    Mission = model.Mission,
                    PasswordExpiryMonths = model.PasswordExpiryMonths,
                    PostalAddress = model.PostalAddress,
                    Town = model.Town,
                    EmailAddress = model.EmailAddress,
                    PrimaryColor = model.PrimaryColor,
                    SecondaryColor = model.SecondaryColor,
                    PrimaryTelephone = model.PrimaryTelephone,
                    SecondaryTelephone = model.SecondaryTelephone,
                    PhoneNumberLength = model.PhoneNumberLength,
                    TelephoneCode = model.TelephoneCode,
                    DisplayBuyingPrice = model.DisplayBuyingPrice,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    LogoPath = model.LogoPath,
                    Notes = model.Notes
                };
                client.ClientId = client.InstanceId = client.Id;
                var instance = new ClientInstance
                {
                    Id = Guid.NewGuid(),
                    ClientId = client.Id,
                    Code = "MAIN",
                    Name = $"MAIN - {model.Name}",
                    Personnel = model.Personnel,
                    Status = EntityStatus.Active
                };
                instance.InstanceId = instance.Id;
                _context.Clients.Add(client);
                _context.ClientInstances.Add(instance);
                await _context.SaveChangesAsync();
                SeedDefaultData(client.Id, instance.Id);

                result.Success = true;
                result.Message = "Added";
                _logger.LogInformation($"{tag} added {client.Name}  {client.Id} : {result.Message}");
                result.Data = client;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public void SeedDefaultData(Guid clientId, Guid instanceId)
        {
            if (!_context.Customers.Any(c => c.ClientId.Equals(clientId)))
            {
                var customer = new Customer
                {
                    Code = Constants.WalkInCustomerCode,
                    FirstName = "WALK IN",
                    Gender = "----",
                    PhoneNumber = "0000000000",
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Personnel = Constants.SuperAdminEmail
                };
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }

            if (!_context.ExpenseTypes.Any(c => c.ClientId.Equals(clientId)))
            {
                var expenseType = new ExpenseType
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Name = "DEFAULT TYPE",
                    Code = "DEFAULT",
                    Personnel = Constants.SuperAdminEmail
                };
                _context.ExpenseTypes.Add(expenseType);
                _context.SaveChanges();
            }

            if (!_context.PaymentModes.Any(c => c.ClientId.Equals(clientId)))
            {
                var payment = new PaymentMode
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Name = Constants.DefaultPaymentMode,
                    Code = Constants.DefaultPaymentMode,
                    Personnel = Constants.SuperAdminEmail
                };
                _context.PaymentModes.Add(payment);
                _context.SaveChanges();
            }

            if (!_context.ProductCategories.Any(c => c.ClientId.Equals(clientId)))
            {
                var productCategory = new ProductCategory
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Name = "DEFAULT",
                    Code = "DEFAULT",
                    Personnel = Constants.SuperAdminEmail
                };
                _context.ProductCategories.Add(productCategory);
                _context.SaveChanges();
            }

            if (!_context.UnitOfMeasures.Any(c => c.ClientId.Equals(clientId)))
            {
                var unitOfMeasure = new UnitOfMeasure
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Name = "PIECES",
                    Code = "PIECES",
                    Personnel = Constants.SuperAdminEmail
                };
                _context.UnitOfMeasures.Add(unitOfMeasure);
                _context.SaveChanges();
            }

            if (!_context.TaxTypes.Any(c => c.ClientId.Equals(clientId)))
            {
                var taxType = new TaxType
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Name = "NONE",
                    Code = "NONE",
                    Personnel = Constants.SuperAdminEmail
                };
                _context.TaxTypes.Add(taxType);
                _context.SaveChanges();
            }

            if (!_context.Products.Any(c => c.ClientId.Equals(clientId)))
            {
                var product = new Product
                {
                    Code = "D000",
                    Name = "DEFAULT",
                    ProductCategoryId = _context.ProductCategories
                    .Where(u => u.ClientId.Equals(clientId)).Select(u => u.Id)
                    .FirstOrDefault(),
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Personnel = Constants.SuperAdminEmail,
                    Status = EntityStatus.Inactive
                };
                product.ProductInstanceStamp = $"{product.Code}_{instanceId}";
                _context.Products.Add(product);
                _context.SaveChanges();
            }

            if (!_context.SmsSettings.Any(c => c.ClientId.Equals(clientId)))
            {
                var setting = new SmsSetting
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Code = "DEFAULT",
                    Personnel = Constants.SuperAdminEmail
                };
                _context.SmsSettings.Add(setting);
                _context.SaveChanges();
            }

            if (!_context.Banks.Any(c => c.ClientId.Equals(clientId)))
            {
                var bank = new Bank
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Code = "000",
                    Name = "DEFAULT",
                    Personnel = Constants.SuperAdminEmail
                };
                _context.Banks.Add(bank);
                _context.SaveChanges();
            }

            if (!_context.Counties.Any(c => c.ClientId.Equals(clientId)))
            {
                var county = new County
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Code = "000",
                    Name = "DEFAULT",
                    Personnel = Constants.SuperAdminEmail
                };
                _context.Counties.Add(county);
                _context.SaveChanges();
            }

            if (!_context.EmployeeKinRelationships.Any(c => c.ClientId.Equals(clientId)))
            {
                var relationship = new EmployeeKinRelationship
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Code = "000",
                    Name = "FATHER",
                    Personnel = Constants.SuperAdminEmail
                };
                _context.EmployeeKinRelationships.Add(relationship);
                _context.SaveChanges();
            }

            if (!_context.Suppliers.Any(c => c.ClientId.Equals(clientId)))
            {
                var supplier = new Supplier
                {
                    ClientId = clientId,
                    InstanceId = instanceId,
                    Code = "000",
                    Name = "DEFAULT",
                    Personnel = Constants.SuperAdminEmail
                };
                _context.Suppliers.Add(supplier);
                _context.SaveChanges();
            }
        }

        public async Task<ReturnData<EmailSetting>> UpdateEmailSettingAsync(EmailSettingViewModel model)
        {
            var result = new ReturnData<EmailSetting> { Data = new EmailSetting() };
            var tag = nameof(UpdateEmailSettingAsync);
            _logger.LogInformation($"{tag} email settings for client {model.ClientId}");
            try
            {
                var dbSetting = await _context.EmailSettings
                    .FirstOrDefaultAsync(e => e.ClientId.Equals(model.ClientId));
                if (dbSetting == null)
                {
                    var setting = new EmailSetting
                    {
                        SmtpServer = model.SmtpServer,
                        SmtpPort = model.SmtpPort,
                        SmtpUsername = model.SmtpUsername,
                        SmtpPassword = model.SmtpPassword,
                        SenderFromName = model.SenderFromName,
                        SenderFromEmail = model.SenderFromEmail,
                        SocketOptions = model.SocketOptions,
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId,
                        Personnel = model.Personnel,
                        Notes = model.Notes
                    };
                    _context.EmailSettings.Add(setting);
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Added";
                    _logger.LogInformation($"{tag} email settings for client {model.ClientId} : {result.Message}");
                    result.Data = setting;
                    return result;
                }
                dbSetting.SmtpServer = model.SmtpServer;
                dbSetting.SmtpPort = model.SmtpPort;
                dbSetting.SmtpUsername = model.SmtpUsername;
                dbSetting.SmtpPassword = model.SmtpPassword;
                dbSetting.SenderFromName = model.SenderFromName;
                dbSetting.SenderFromEmail = model.SenderFromEmail;
                dbSetting.SocketOptions = model.SocketOptions;
                dbSetting.LastModifiedBy = model.Personnel;
                dbSetting.DateLastModified = DateTime.Now;
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Updated";
                _logger.LogInformation($"{tag} email settings for client {model.ClientId} : {result.Message}");
                result.Data = dbSetting;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occured";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }
    }
}


