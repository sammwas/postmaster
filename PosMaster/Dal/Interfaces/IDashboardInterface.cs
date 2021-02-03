using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.Services;
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
        Task<ReturnData<ClerkDashboardViewModel>> ClerkDashboardAsync(Guid instanceId, string personnel);
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

        public async Task<ReturnData<ClerkDashboardViewModel>> ClerkDashboardAsync(Guid instanceId, string personnel)
        {
            var result = new ReturnData<ClerkDashboardViewModel> { Data = new ClerkDashboardViewModel() };
            var tag = nameof(ClerkDashboardAsync);
            _logger.LogInformation($"{tag} get clerk {personnel} dashboard figures for instance {instanceId}");
            try
            {
                var firstWeekDay = Helpers.FirstDayOfWeek();
                var lastWeekDay = firstWeekDay.AddDays(6);
                var data = new ClerkDashboardViewModel
                {
                    TodayExpenses = await _context.Expenses
                    .Where(c => c.InstanceId.Equals(instanceId) && c.Personnel.Equals(personnel)
                      && c.DateCreated.Date.Equals(DateTime.Now.Date)).SumAsync(c => c.Amount),
                    TodayTotalExpenses = await _context.Expenses
                    .Where(c => c.InstanceId.Equals(instanceId)
                      && c.DateCreated.Date.Equals(DateTime.Now.Date)).SumAsync(c => c.Amount),
                    TodaySales = await _context.ReceiptLineItems
                    .Where(c => c.InstanceId.Equals(instanceId) && c.Personnel.Equals(personnel)
                      && c.DateCreated.Date.Equals(DateTime.Now.Date)).SumAsync(c => c.Amount),
                    TodayTotalSales = await _context.ReceiptLineItems
                    .Where(c => c.InstanceId.Equals(instanceId)
                      && c.DateCreated.Date.Equals(DateTime.Now.Date)).SumAsync(c => c.Amount),
                    WeeklySales = await _context.ReceiptLineItems
                    .Where(c => c.InstanceId.Equals(instanceId) && c.Personnel.Equals(personnel)
                      && c.DateCreated.Date >= firstWeekDay.Date && c.DateCreated <= lastWeekDay.Date).SumAsync(c => c.Amount),
                    WeeklyTotalSales = await _context.ReceiptLineItems
                    .Where(c => c.InstanceId.Equals(instanceId)
                      && c.DateCreated.Date >= firstWeekDay.Date && c.DateCreated <= lastWeekDay.Date).SumAsync(c => c.Amount),
                    MonthlySales = await _context.ReceiptLineItems
                    .Where(c => c.InstanceId.Equals(instanceId) && c.Personnel.Equals(personnel)
                    && c.DateCreated.Date >= Helpers.firstDayOfMonth.Date && c.DateCreated <= Helpers.lastDayOfMonth.Date).SumAsync(c => c.Amount),
                    MonthlyTotalSales = await _context.ReceiptLineItems.Where(c => c.InstanceId.Equals(instanceId)
                    && c.DateCreated.Date >= Helpers.firstDayOfMonth.Date && c.DateCreated <= Helpers.lastDayOfMonth.Date).SumAsync(c => c.Amount)
                };
                result.Success = true;
                result.Message = "Found";
                result.Data = data;
                _logger.LogInformation($"{tag} clerk {personnel} dashboard {result.Message} : today sales  {data.TodaySales}");
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

        public async Task<ReturnData<ManagerDashboardViewModel>> ManagerDashboardAsync(Guid clientId)
        {
            var result = new ReturnData<ManagerDashboardViewModel> { Data = new ManagerDashboardViewModel() };
            var tag = nameof(ManagerDashboardAsync);
            _logger.LogInformation($"{tag} get manager dashboard figures");
            try
            {
                var firstWeekDay = Helpers.FirstDayOfWeek();
                var lastWeekDay = firstWeekDay.AddDays(6);
                var data = new ManagerDashboardViewModel
                {
                    TodaySales = await _context.ReceiptLineItems.Where(c => c.ClientId.Equals(clientId)
                      && c.DateCreated.Date.Equals(DateTime.Now.Date)).SumAsync(c => c.Amount),
                    WeeklySales = await _context.ReceiptLineItems.Where(c => c.ClientId.Equals(clientId)
                      && c.DateCreated.Date >= firstWeekDay.Date && c.DateCreated <= lastWeekDay.Date).SumAsync(c => c.Amount),
                    MonthlySales = await _context.ReceiptLineItems.Where(c => c.ClientId.Equals(clientId)
                    && c.DateCreated.Date >= Helpers.firstDayOfMonth.Date && c.DateCreated <= Helpers.lastDayOfMonth.Date).SumAsync(c => c.Amount),
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
                var x = await _context.ReceiptLineItems.SumAsync(r => r.ActualProfit);
                var data = new SuperAdminDashboardViewModel
                {
                    Clients = await _context.Clients.CountAsync(),
                    ClientInstances = await _context.ClientInstances.CountAsync(),
                    Users = await _context.Users.CountAsync(),
                    Products = await _context.Products.CountAsync(),
                    TotalStockValue = await _context.Products.Where(p => p.Status.Equals(EntityStatus.Active)).SumAsync(p => p.SellingPrice),
                    TotalActualProfit = 0,
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
