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
    public interface ICustomerInterface
    {
        Task<ReturnData<Customer>> EditAsync(CustomerViewModel model);
        Task<ReturnData<List<Customer>>> AllAsync();
        Task<ReturnData<List<Customer>>> ByClientIdAsync(Guid clientId);
        Task<ReturnData<List<Customer>>> ByInstanceIdAsync(Guid instanceId);
        Task<ReturnData<Customer>> ByIdAsync(Guid id);
        Task<ReturnData<FormSelectViewModel>> DefaultClientCustomerAsync(Guid clientId, string customerId = "");
        Task<ReturnData<List<FormSelectViewModel>>> SearchClientCustomerAsync(Guid clientId, string term, int limit = 25);
        Task<ReturnData<ReceiptUserViewModel>> GlUserBalanceAsync(GlUserType userType, Guid userId);
    }

    public class CustomerImplementation : ICustomerInterface
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CustomerImplementation> _logger;
        private readonly IProductInterface _productInterface;
        public CustomerImplementation(DatabaseContext context, ILogger<CustomerImplementation> logger,
            IProductInterface productInterface)
        {
            _context = context;
            _logger = logger;
            _productInterface = productInterface;
        }
        public async Task<ReturnData<List<Customer>>> AllAsync()
        {
            var result = new ReturnData<List<Customer>> { Data = new List<Customer>() };
            var tag = nameof(AllAsync);
            _logger.LogInformation($"{tag} get all Customers");
            try
            {
                var data = await _context.Customers
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} Customers");
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

        public async Task<ReturnData<List<Customer>>> ByClientIdAsync(Guid clientId)
        {
            var result = new ReturnData<List<Customer>> { Data = new List<Customer>() };
            var tag = nameof(ByClientIdAsync);
            _logger.LogInformation($"{tag} get Customers for client {clientId}");
            try
            {
                var data = await _context.Customers
                    .Where(c => c.ClientId.Equals(clientId))
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} Customers");
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

        public async Task<ReturnData<Customer>> ByIdAsync(Guid id)
        {
            var result = new ReturnData<Customer> { Data = new Customer() };
            var tag = nameof(ByIdAsync);
            _logger.LogInformation($"{tag} get Customer by id - {id}");
            try
            {
                var data = await _context.Customers.FirstOrDefaultAsync(c => c.Id.Equals(id));
                result.Success = data != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} Customer {id} {result.Message}");
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

        public async Task<ReturnData<List<Customer>>> ByInstanceIdAsync(Guid instanceId)
        {
            var result = new ReturnData<List<Customer>> { Data = new List<Customer>() };
            var tag = nameof(ByInstanceIdAsync);
            _logger.LogInformation($"{tag} get all instance {instanceId} customers");
            try
            {
                var data = await _context.Customers
                    .Where(c => c.InstanceId.Equals(instanceId))
                    .OrderByDescending(c => c.FirstName)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} customers");
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

        public async Task<ReturnData<FormSelectViewModel>> DefaultClientCustomerAsync(Guid clientId, string customerId = "")
        {
            var result = new ReturnData<FormSelectViewModel> { Data = new FormSelectViewModel() };
            var tag = nameof(DefaultClientCustomerAsync);
            _logger.LogInformation($"{tag} get client customer by client-id - {clientId} customer {customerId}");
            try
            {
                var hasId = Guid.TryParse(customerId, out var id);
                var dataQry = _context.Customers
                    .Where(c => c.ClientId.Equals(clientId));
                dataQry = hasId ?
                    dataQry.Where(c => c.Id.Equals(id)) : dataQry.Where(c => c.Code.Equals(Constants.WalkInCustomerCode));
                var data = await dataQry.FirstOrDefaultAsync();
                result.Success = data != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = new FormSelectViewModel(data);
                _logger.LogInformation($"{tag} Customer for {clientId} {result.Message}");
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

        public async Task<ReturnData<Customer>> EditAsync(CustomerViewModel model)
        {
            var result = new ReturnData<Customer> { Data = new Customer() };
            var tag = nameof(EditAsync);
            _logger.LogInformation($"{tag} edit Customer {model.FirstName}");
            try
            {
                if (model.IsEditMode)
                {
                    var dbCustomer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbCustomer == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogWarning($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbCustomer.Code = model.Code;
                    dbCustomer.FirstName = model.FirstName;
                    dbCustomer.LastName = model.LastName;
                    dbCustomer.PhoneNumber = model.PhoneNumber;
                    dbCustomer.EmailAddress = model.EmailAddress;
                    dbCustomer.Website = model.Website;
                    dbCustomer.Location = model.Location;
                    dbCustomer.Town = model.Town;
                    dbCustomer.PostalAddress = model.PostalAddress;
                    dbCustomer.LastModifiedBy = model.Personnel;
                    dbCustomer.DateLastModified = DateTime.Now;
                    dbCustomer.Notes = model.Notes;
                    dbCustomer.Status = model.Status;
                    dbCustomer.CreditLimit = model.CreditLimit;
                    dbCustomer.Gender = model.Gender;
                    dbCustomer.PinNo = model.PinNo;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbCustomer;
                    _logger.LogInformation($"{tag} updated {dbCustomer.FirstName} {model.Id} : {result.Message}");
                    return result;
                }
                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Code = model.Code,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    PhoneNumber = model.PhoneNumber,
                    EmailAddress = model.EmailAddress,
                    PostalAddress = model.PostalAddress,
                    Location = model.Location,
                    Town = model.Town,
                    Website = model.Website,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CreditLimit = model.CreditLimit,
                    Gender = model.Gender,
                    PinNo = model.PinNo
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                #region OPENING_BALANCE
                var paymentModeId = _context.PaymentModes.Where(x => x.ClientId == model.ClientId)
                    .OrderByDescending(p => p.DateCreated)
                    .Select(p => p.Id)
                    .FirstOrDefault();
                if (model.OpeningBalance < 0)
                {
                    var rcptModel = new ReceiptUserViewModel
                    {
                        UserId = customer.Id.ToString(),
                        UserType = GlUserType.Customer,
                        Amount = Math.Abs(model.OpeningBalance),
                        Notes = $"Opening balance {model.OpeningBalanceAsAt}",
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId,
                        Code = model.Code,
                        PaymentModeId = paymentModeId.ToString(),
                        Personnel = model.Personnel,
                    };
                    await _productInterface.ReceiptExcessAmount(rcptModel);
                }

                if (model.OpeningBalance > 0)
                {
                    var productId = _context.Products
                        .Where(p => p.ClientId.Equals(model.ClientId))
                        .Select(p => p.Id)
                        .FirstOrDefault();
                    if (!productId.Equals(Guid.Empty))
                    {
                        var rcptId = Guid.NewGuid();
                        var rCode = _productInterface.DocumentRefNumber(Document.Receipt, model.ClientId);
                        var receipt = new Receipt
                        {
                            Id = rcptId,
                            CustomerId = customer.Id,
                            IsWalkIn = false,
                            IsCredit = true,
                            Code = rCode,
                            ClientId = model.ClientId,
                            InstanceId = model.InstanceId,
                            Personnel = model.Personnel,
                            ReceiptLineItems = new List<ReceiptLineItem> {
                        new ReceiptLineItem
                        {
                            ClientId=model.ClientId,
                            InstanceId=model.InstanceId,
                            Quantity=1,
                            UnitPrice=model.OpeningBalance,
                            Notes=$"Opening balance -{rCode}",
                            ReceiptId=rcptId,
                            Personnel=model.Personnel,
                            ProductId=productId,
                        }  }
                        };
                        _context.Receipts.Add(receipt);
                        await _context.SaveChangesAsync();
                        await _productInterface.AddInvoiceAsync(receipt);
                    }
                }

                #endregion

                result.Success = true;
                result.Message = "Added";
                result.Data = customer;
                _logger.LogInformation($"{tag} added {customer.FirstName} {customer.Code} - {customer.Id} : {result.Message}");
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

        public async Task<ReturnData<ReceiptUserViewModel>> GlUserBalanceAsync(GlUserType userType, Guid userId)
        {
            var tag = nameof(GlUserBalanceAsync);
            _logger.LogInformation($"{tag} get {userType} {userId}  balance");
            var result = new ReturnData<ReceiptUserViewModel>
            {
                Data = new ReceiptUserViewModel
                {
                    UserId = userId.ToString(),
                    UserType = userType
                }
            };
            try
            {
                var debit = await _context.GeneralLedgerEntries
                    .Where(u => u.UserId.Equals(userId))
                    .SumAsync(l => l.Debit);
                var credit = await _context.GeneralLedgerEntries
                    .Where(u => u.UserId.Equals(userId))
                    .SumAsync(l => l.Credit);
                result.Success = true;
                result.Message = "Found";
                result.Data.CreditAmount = credit;
                result.Data.DebitAmount = debit;
                _logger.LogInformation($"{tag} {result.Message} : credit= {credit} debit= {debit}");
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

        public async Task<ReturnData<List<FormSelectViewModel>>> SearchClientCustomerAsync(Guid clientId, string term, int limit = 25)
        {
            var result = new ReturnData<List<FormSelectViewModel>> { Data = new List<FormSelectViewModel>() };
            var tag = nameof(SearchClientCustomerAsync);
            _logger.LogInformation($"{tag} get Customers for client - {clientId} search -{term}");
            try
            {
                var dataQry = _context.Customers
                    .Where(c => c.ClientId.Equals(clientId))
                    .OrderByDescending(c => c.DateLastModified)
                    .AsQueryable();
                if (string.IsNullOrEmpty(term))
                {
                    result.Message = "Term is required";
                    return result;
                }
                term = term.ToLower();
                var data = await dataQry
                    .Where(c => c.Code.ToLower().Contains(term) ||
                     c.FirstName.ToLower().Contains(term) || c.LastName.ToLower().Contains(term) ||
                     c.PhoneNumber.Contains(term) || c.IdNumber.Contains(term))
                    .Select(c => new FormSelectViewModel(c))
                    .Take(limit).ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} Customer {clientId} {result.Message} -{data.Count} records");
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
