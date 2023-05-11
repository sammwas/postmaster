using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.Services;
using PosMaster.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
    public interface IDashboardInterface
    {
        Task<ReturnData<SuperAdminDashboardViewModel>> SuperAdminDashboardAsync(Guid instanceId, DateTime dtFrom, DateTime dateTo);
        Task<ReturnData<ManagerDashboardViewModel>> ManagerDashboardAsync(Guid instanceId, DateTime dtFrom, DateTime dateTo);
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
                    TodaySales = await _context.ReceiptLineItems
                    .Where(c => c.InstanceId.Equals(instanceId) && c.Personnel.Equals(personnel)
                      && c.DateCreated.Date.Equals(DateTime.Now.Date))
                    .SumAsync(c => c.UnitPrice * c.Quantity),
                    CashSales = await _context.Receipts
                    .Where(c => c.InstanceId.Equals(instanceId) && c.Personnel.Equals(personnel)
                      && c.DateCreated.Date.Equals(DateTime.Now.Date))
                    .SumAsync(c => c.AmountReceived),
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

        public async Task<ReturnData<ManagerDashboardViewModel>> ManagerDashboardAsync(Guid instanceId, DateTime dateFrom, DateTime dateTo)
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

                    TotalStockValue = await _context.ProductPoQuantityLogs.Where(p => p.InstanceId.Equals(instanceId))
                    .SumAsync(p => p.BuyingPrice * p.AvailableQuantity),

                    TodaySales = await _context.ReceiptLineItems.Where(c => c.InstanceId.Equals(instanceId)
                      && c.DateCreated.Date.Equals(DateTime.Now.Date)).SumAsync(c => c.UnitPrice * c.Quantity),

                    WeeklySales = await _context.ReceiptLineItems.Where(c => c.InstanceId.Equals(instanceId)
                      && c.DateCreated.Date >= firstWeekDay.Date && c.DateCreated.Date <= lastWeekDay.Date).SumAsync(c => c.UnitPrice * c.Quantity),

                    MonthlySales = await _context.ReceiptLineItems.Where(c => c.InstanceId.Equals(instanceId)
                    && c.DateCreated.Date >= Helpers.firstDayOfMonth.Date && c.DateCreated.Date <= Helpers.lastDayOfMonth.Date)
                    .SumAsync(c => c.UnitPrice * c.Quantity),

                    TotalActualProfit = await _context.ReceiptLineItems.Where(p => p.InstanceId.Equals(instanceId))
                    .Where(r => r.DateCreated.Date >= dateFrom.Date && r.DateCreated.Date <= dateTo.Date)
                    .SumAsync(r => (r.UnitPrice * r.Quantity) - (r.BuyingPrice * r.Quantity)),

                    TotalExpectedProfit = await _context.ReceiptLineItems.Where(c => c.InstanceId.Equals(instanceId))
                    .Where(r => r.DateCreated.Date >= dateFrom.Date && r.DateCreated.Date <= dateTo.Date)
                    .SumAsync(r => (r.SellingPrice * r.Quantity) - (r.BuyingPrice * r.Quantity)),

                    TotalReceiptsAmount = await _context.ReceiptLineItems.Where(c => c.InstanceId.Equals(instanceId))
                    .Where(r => r.DateCreated.Date >= dateFrom.Date && r.DateCreated.Date <= dateTo.Date)
                    .SumAsync(r => r.UnitPrice * r.Quantity),

                    TotalExpenses = await _context.Expenses
                     .Where(r => r.DateCreated.Date >= dateFrom.Date && r.DateCreated.Date <= dateTo.Date)
                    .SumAsync(e => e.Amount),

                    SupplierExpenses = await _context.Expenses
                    .Where(s => s.SupplierId.HasValue)
                     .Where(r => r.DateCreated.Date >= dateFrom.Date && r.DateCreated.Date <= dateTo.Date)
                    .SumAsync(e => e.Amount)
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

        public async Task<ReturnData<SuperAdminDashboardViewModel>> SuperAdminDashboardAsync(Guid instanceId, DateTime dtFrom, DateTime dateTo)
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
                    TotalStockValue = await _context.ProductPoQuantityLogs
                    .SumAsync(p => p.BuyingPrice * p.AvailableQuantity),

                    TotalActualProfit = await _context.ReceiptLineItems
                    .SumAsync(r => (r.UnitPrice * r.Quantity) - (r.BuyingPrice * r.Quantity)),

                    TotalExpectedProfit = await _context.ReceiptLineItems.SumAsync(r =>
                    (r.SellingPrice * r.Quantity) - (r.BuyingPrice * r.Quantity)),
                    TotalReceiptsAmount = await _context.ReceiptLineItems.SumAsync(r => r.Quantity * r.UnitPrice),
                    DailySalesList = await _context.ReceiptLineItems.Where(d => d.DateCreated >= DateTime.Now.AddDays(-7))
                    .OrderBy(s => s.DateCreated).GroupBy(r => r.DateCreated.Date, (d, t)
                    => new ChartData { Day = d.ToString("dddd"), Amount = t.Sum(r => r.Quantity * r.UnitPrice) })
                    .ToListAsync(),
                    TotalExpenses = await _context.Expenses.SumAsync(e => e.Amount)
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
