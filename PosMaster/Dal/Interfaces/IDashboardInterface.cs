using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
	public interface IDashboardInterface
	{
		Task<ReturnData<SuperAdminDashboardViewModel>> SuperAdminDashboardAsync();
		Task<ReturnData<ManagerDashboardViewModel>> ManagerDashboardAsync(Guid clientId);
	}

	public class DashboardImplementation : IDashboardInterface
	{
		private readonly DatabaseContext _context;
		private readonly ILogger<DashboardImplementation> _logger;
		public DashboardImplementation(DatabaseContext context, ILogger<DashboardImplementation> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<ReturnData<ManagerDashboardViewModel>> ManagerDashboardAsync(Guid clientId)
		{
			var result = new ReturnData<ManagerDashboardViewModel> { Data = new ManagerDashboardViewModel() };
			var tag = nameof(ManagerDashboardAsync);
			_logger.LogInformation($"{tag} get manager dashboard figures");
			try
			{
				var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
				var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
				var dayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
				int delta = dayOfWeek - DateTime.Now.DayOfWeek;
				var firstWeekDay = DateTime.Now.AddDays(delta);
				var lastWeekDay = firstWeekDay.AddDays(6);
				var data = new ManagerDashboardViewModel
				{
					TodaySales = await _context.ReceiptLineItems.Where(c => c.ClientId.Equals(clientId)
					  && c.DateCreated.Date.Equals(DateTime.Now.Date)).SumAsync(c => c.Amount),
					WeeklySales = await _context.ReceiptLineItems.Where(c => c.ClientId.Equals(clientId)
					  && c.DateCreated.Date >= firstWeekDay.Date && c.DateCreated <= lastWeekDay.Date).SumAsync(c => c.Amount),
					MonthlySales = await _context.ReceiptLineItems.Where(c => c.ClientId.Equals(clientId)
					&& c.DateCreated.Date >= firstDayOfMonth.Date && c.DateCreated <= lastDayOfMonth.Date).SumAsync(c => c.Amount),
					Products = await _context.Products.CountAsync(p => p.ClientId.Equals(clientId)),
					TotalStockValue = await _context.Products.Where(p => p.ClientId.Equals(clientId)).SumAsync(p => p.TotalValue),
					TotalActualProfit = await _context.ReceiptLineItems.Where(p => p.ClientId.Equals(clientId)).SumAsync(r => r.ActualProfit),
					TotalExpectedProfit = await _context.ReceiptLineItems.Where(c => c.ClientId.Equals(clientId)).SumAsync(r => r.ExpectedProfit),
					TotalReceiptsAmount = await _context.ReceiptLineItems.Where(c => c.ClientId.Equals(clientId)).SumAsync(r => r.Amount)
				};
				result.Success = true;
				result.Message = "Found";
				result.Data = data;
				_logger.LogInformation($"{tag} manager dashboard {result.Message} : receipts total {data.TotalReceiptsAmount}");
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

		public async Task<ReturnData<SuperAdminDashboardViewModel>> SuperAdminDashboardAsync()
		{
			var result = new ReturnData<SuperAdminDashboardViewModel> { Data = new SuperAdminDashboardViewModel() };
			var tag = nameof(SuperAdminDashboardAsync);
			_logger.LogInformation($"{tag} get superadmin dashboard figures");
			try
			{
				var data = new SuperAdminDashboardViewModel
				{
					Clients = await _context.Clients.CountAsync(),
					ClientInstances = await _context.ClientInstances.CountAsync(),
					Users = await _context.Users.CountAsync(),
					Products = await _context.Products.CountAsync(),
					TotalStockValue = await _context.Products.SumAsync(p => p.TotalValue),
					TotalActualProfit = await _context.ReceiptLineItems.SumAsync(r => r.ActualProfit),
					TotalExpectedProfit = await _context.ReceiptLineItems.SumAsync(r => r.ExpectedProfit),
					TotalReceiptsAmount = await _context.ReceiptLineItems.SumAsync(r => r.Amount)
				};
				result.Success = true;
				result.Message = "Found";
				result.Data = data;
				_logger.LogInformation($"{tag} superadmin dashboard {result.Message} : receipts total {data.TotalReceiptsAmount}");
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
