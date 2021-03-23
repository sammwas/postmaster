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
		Task<ReturnData<EmployeeLeaveCategory>> EditLeaveCategoryAsync(EmployeeLeaveCategoryViewModel model);
		Task<ReturnData<EmployeeLeaveCategory>> LeaveCategoryByIdAsync(Guid id);
		Task<ReturnData<List<EmployeeLeaveCategory>>> LeaveCategoriesAsync(Guid clientId);
		Task<ReturnData<EmployeeSalary>> EditEmployeeSalaryAsync(EmployeeSalaryViewModel model);
		Task<ReturnData<EmployeeSalary>> EmployeeSalaryByIdAsync(string userId);
		Task<ReturnData<List<EmployeeSalary>>> EmployeeSalariesAsync(Guid clientId);
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

		public async Task<ReturnData<EmployeeSalary>> EditEmployeeSalaryAsync(EmployeeSalaryViewModel model)
		{
			var result = new ReturnData<EmployeeSalary> { Data = new EmployeeSalary() };
			var tag = nameof(EditEmployeeSalaryAsync);
			_logger.LogInformation($"{tag} edit employee salary for {model.UserId}");
			try
			{

				var salary = await _context.EmployeeSalaries
					.FirstOrDefaultAsync(c => c.UserId.Equals(model.UserId));
				if (salary == null)
				{
					var empSalary = new EmployeeSalary
					{
						ClientId = model.ClientId,
						InstanceId = model.InstanceId,
						UserId = model.UserId,
						Allowance = model.Allowance,
						Deduction = model.Deduction,
						BasicPay = model.BasicPay,
						Bank = model.Bank,
						BankAccount = model.BankAccount,
						Personnel = model.Personnel,
						Status = model.Status,
						Notes = model.Notes
					};
					_context.EmployeeSalaries.Add(empSalary);
					await _context.SaveChangesAsync();
					result.Success = true;
					result.Message = "Added";
					result.Data = empSalary;
					return result;
				}
				var prevNet = salary.NetAmount;
				salary.Code = model.Code;
				salary.Allowance = model.Allowance;
				salary.BasicPay = model.BasicPay;
				salary.Deduction = model.Deduction;
				salary.Bank = model.Bank;
				salary.BankAccount = model.BankAccount;
				salary.LastModifiedBy = model.Personnel;
				salary.DateLastModified = DateTime.Now;
				salary.Notes = model.Notes;
				salary.Status = model.Status;
				if (prevNet != salary.NetAmount)
				{
					var salaryLog = new EmployeeSalaryLog
					{
						ClientId = model.ClientId,
						InstanceId = model.InstanceId,
						UserId = model.UserId,
						Personnel = model.Personnel,
						SalaryFrom = prevNet,
						SalaryTo = salary.NetAmount
					};
					_context.EmployeeSalaryLogs.Add(salaryLog);
				}
				await _context.SaveChangesAsync();
				result.Success = true;
				result.Message = "Updated";
				result.Data = salary;
				_logger.LogInformation($"{tag} updated {salary.Code} {model.Id} : {result.Message}");
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

		public async Task<ReturnData<EmployeeLeaveCategory>> EditLeaveCategoryAsync(EmployeeLeaveCategoryViewModel model)
		{
			var result = new ReturnData<EmployeeLeaveCategory> { Data = new EmployeeLeaveCategory() };
			var tag = nameof(EditLeaveCategoryAsync);
			_logger.LogInformation($"{tag} edit leave category");
			try
			{
				if (model.IsEditMode)
				{
					var dbCategory = await _context.EmployeeLeaveCategories
						.FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
					if (dbCategory == null)
					{
						result.Message = "Not Found";
						_logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
						return result;
					}
					dbCategory.Title = model.Title;
					dbCategory.Code = model.Code;
					dbCategory.MaxDays = model.MaxDays;
					dbCategory.AllowedGender = model.FemaleOnly ? "Female" : "";
					dbCategory.LastModifiedBy = model.Personnel;
					dbCategory.DateLastModified = DateTime.Now;
					dbCategory.Notes = model.Notes;
					dbCategory.Status = model.Status;
					await _context.SaveChangesAsync();
					result.Success = true;
					result.Message = "Updated";
					result.Data = dbCategory;
					_logger.LogInformation($"{tag} updated {dbCategory.Title} {model.Id} : {result.Message}");
					return result;
				}

				var category = new EmployeeLeaveCategory
				{
					Title = model.Title,
					Notes = model.Notes,
					ClientId = model.ClientId,
					Personnel = model.Personnel,
					Status = model.Status,
					Code = model.Code,
					MaxDays = model.MaxDays,
					AllowedGender = model.FemaleOnly ? "Female" : "",
					InstanceId = model.InstanceId
				};
				_context.EmployeeLeaveCategories.Add(category);
				await _context.SaveChangesAsync();
				result.Success = true;
				result.Message = "Added";
				result.Data = category;
				_logger.LogInformation($"{tag} added {category.Title}  {category.Id} : {result.Message}");
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

		public async Task<ReturnData<List<EmployeeSalary>>> EmployeeSalariesAsync(Guid clientId)
		{
			var result = new ReturnData<List<EmployeeSalary>> { Data = new List<EmployeeSalary>() };
			var tag = nameof(EmployeeSalariesAsync);
			_logger.LogInformation($"{tag} get client employee salaries");
			try
			{
				var data = await _context.EmployeeSalaries.Include(s => s.User)
					.Where(b => b.ClientId.Equals(clientId))
					.OrderByDescending(c => c.DateCreated)
					.ToListAsync();
				result.Success = data.Any();
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = data;
				_logger.LogInformation($"{tag} found {data.Count} employee salary records");
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

		public async Task<ReturnData<EmployeeSalary>> EmployeeSalaryByIdAsync(string userId)
		{
			var result = new ReturnData<EmployeeSalary> { Data = new EmployeeSalary() };
			var tag = nameof(EmployeeSalaryByIdAsync);
			_logger.LogInformation($"{tag} get employee {userId} salary");
			try
			{
				var salary = await _context.EmployeeSalaries
					.FirstOrDefaultAsync(c => c.UserId.Equals(userId));
				result.Success = salary != null;
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = salary;
				_logger.LogInformation($"{tag} salary record for {userId} {result.Message}");
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

		public async Task<ReturnData<List<EmployeeLeaveCategory>>> LeaveCategoriesAsync(Guid clientId)
		{
			var result = new ReturnData<List<EmployeeLeaveCategory>> { Data = new List<EmployeeLeaveCategory>() };
			var tag = nameof(LeaveCategoriesAsync);
			_logger.LogInformation($"{tag} get all client leave categories");
			try
			{
				var data = await _context.EmployeeLeaveCategories
					.Where(b => b.ClientId.Equals(clientId))
					.OrderByDescending(c => c.DateCreated)
					.ToListAsync();
				result.Success = data.Any();
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = data;
				_logger.LogInformation($"{tag} found {data.Count} client leave categories");
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

		public async Task<ReturnData<EmployeeLeaveCategory>> LeaveCategoryByIdAsync(Guid id)
		{
			var result = new ReturnData<EmployeeLeaveCategory> { Data = new EmployeeLeaveCategory() };
			var tag = nameof(LeaveCategoryByIdAsync);
			_logger.LogInformation($"{tag} get leave category by id - {id}");
			try
			{
				var category = await _context.EmployeeLeaveCategories
					.FirstOrDefaultAsync(c => c.Id.Equals(id));
				result.Success = category != null;
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = category;
				_logger.LogInformation($"{tag} leave category {id} {result.Message}");
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
