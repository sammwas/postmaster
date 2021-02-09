using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
	}

	public class CustomerImplementation : ICustomerInterface
	{
		private readonly DatabaseContext _context;
		private readonly ILogger<CustomerImplementation> _logger;
		public CustomerImplementation(DatabaseContext context, ILogger<CustomerImplementation> logger)
		{
			_context = context;
			_logger = logger;
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

		public async Task<ReturnData<Customer>> EditAsync(CustomerViewModel model)
		{
			var result = new ReturnData<Customer> { Data = new Customer() };
			var tag = nameof(EditAsync);
			_logger.LogInformation($"{tag} edit Customer");
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
					await _context.SaveChangesAsync();
					result.Success = true;
					result.Message = "Updated";
					result.Data = dbCustomer;
					_logger.LogInformation($"{tag} updated {dbCustomer.FirstName} {model.Id} : {result.Message}");
					return result;
				}
				var Customer = new Customer
				{
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
					Gender = model.Gender
				};
				_context.Customers.Add(Customer);
				await _context.SaveChangesAsync();
				result.Success = true;
				result.Message = "Added";
				result.Data = Customer;
				_logger.LogInformation($"{tag} added {Customer.FirstName} {Customer.Code}  {Customer.Id} : {result.Message}");
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
