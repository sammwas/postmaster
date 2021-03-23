using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
	public interface IHrInterface
	{
		Task<ReturnData<Bank>> EditBankAsync(BankViewModel model);
		Task<ReturnData<Bank>> BankByIdAsync(Guid id);
		Task<ReturnData<List<Bank>>> BanksAsync(Guid clientId);
	}

	public class HrImplementation : IHrInterface
	{
		private readonly DatabaseContext _context;
		private readonly ILogger<HrImplementation> _logger;
		public HrImplementation(DatabaseContext context, ILogger<HrImplementation> logger)
		{
			_context = context;
			_logger = logger;
		}


		public async Task<ReturnData<Bank>> BankByIdAsync(Guid id)
		{
			var result = new ReturnData<Bank> { Data = new Bank() };
			var tag = nameof(BankByIdAsync);
			_logger.LogInformation($"{tag} get bank by id - {id}");
			try
			{
				var bank = await _context.Banks
					.FirstOrDefaultAsync(c => c.Id.Equals(id));
				result.Success = bank != null;
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = bank;
				_logger.LogInformation($"{tag} bank {id} {result.Message}");
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

		public async Task<ReturnData<List<Bank>>> BanksAsync(Guid clientId)
		{
			var result = new ReturnData<List<Bank>> { Data = new List<Bank>() };
			var tag = nameof(BanksAsync);
			_logger.LogInformation($"{tag} get all client banks");
			try
			{
				var data = await _context.Banks
					.Where(b => b.ClientId.Equals(clientId))
					.OrderByDescending(c => c.DateCreated)
					.ToListAsync();
				result.Success = data.Any();
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = data;
				_logger.LogInformation($"{tag} found {data.Count} client banks");
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

		public async Task<ReturnData<Bank>> EditBankAsync(BankViewModel model)
		{
			var result = new ReturnData<Bank> { Data = new Bank() };
			var tag = nameof(EditBankAsync);
			_logger.LogInformation($"{tag} edit client bank");
			try
			{
				if (model.IsEditMode)
				{
					var dbBank = await _context.Banks
						.FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
					if (dbBank == null)
					{
						result.Message = "Not Found";
						_logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
						return result;
					}
					dbBank.Name = model.Name;
					dbBank.Code = model.Code;
					dbBank.EmailAddress = model.EmailAddress;
					dbBank.PhoneNumber = model.PhoneNumber;
					dbBank.Website = model.Website;
					dbBank.ContactPerson = model.ContactPerson;
					dbBank.LastModifiedBy = model.Personnel;
					dbBank.DateLastModified = DateTime.Now;
					dbBank.Notes = model.Notes;
					dbBank.Status = model.Status;
					await _context.SaveChangesAsync();
					result.Success = true;
					result.Message = "Updated";
					result.Data = dbBank;
					_logger.LogInformation($"{tag} updated {dbBank.Name} {model.Id} : {result.Message}");
					return result;
				}

				var bank = new Bank
				{
					Name = model.Name,
					Website = model.Website,
					PhoneNumber = model.PhoneNumber,
					Notes = model.Notes,
					ClientId = model.ClientId,
					Personnel = model.Personnel,
					Status = model.Status,
					Code = model.Code,
					EmailAddress = model.EmailAddress,
					ContactPerson = model.ContactPerson,
					InstanceId = model.InstanceId
				};
				_context.Banks.Add(bank);
				await _context.SaveChangesAsync();
				result.Success = true;
				result.Message = "Added";
				result.Data = bank;
				_logger.LogInformation($"{tag} added {bank.Name}  {bank.Id} : {result.Message}");
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
