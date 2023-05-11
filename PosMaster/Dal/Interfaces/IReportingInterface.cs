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
    public interface IReportingInterface
    {
        Task<ReturnData<List<DailySalesViewModel>>> DailySalesReportAsync(Guid instanceId, string dateFrom = "", string dateTo = "", string search = "");
        Task<ReturnData<List<MonthSaleViewModel>>> MonthlySalesReportAsync(Guid instanceId, string dateFrom = "", string dateTo = "");
        Task<ReturnData<List<CustomerBalanceViewModel>>> CustomerBalancesAsync(Guid instanceId, string dateFrom = "", string dateTo = "");
        Task<ReturnData<CustomerStatementViewModel>> CustomerStatementAsync(Guid customerId, string dateFrom = "", string dateTo = "");
        Task<ReturnData<List<TopSellingProductViewModel>>> SalesByClerkAsync(Guid instanceId, string dateFrom = "", string dateTo = "");
        Task<ReturnData<CloseOfDayViewModel>> CloseOfDayAsync(Guid clientId, Guid? instanceId, DateTime dateFrom, DateTime dateTo, string personnel = "");

    }
    public class ReportingImplementation : IReportingInterface
    {
        private readonly DatabaseContext _context;
        private readonly ICustomerInterface _customerInterface;
        private readonly IProductInterface _productInterface;
        private readonly ILogger<ReportingImplementation> _logger;
        public ReportingImplementation(DatabaseContext context, ILogger<ReportingImplementation> logger,
            ICustomerInterface customerInterface, IProductInterface productInterface)
        {
            _context = context;
            _logger = logger;
            _customerInterface = customerInterface;
            _productInterface = productInterface;
        }

        public async Task<ReturnData<List<DailySalesViewModel>>> DailySalesReportAsync(Guid instanceId, string dateFrom = "", string dateTo = "", string search = "")
        {
            var result = new ReturnData<List<DailySalesViewModel>> { Data = new List<DailySalesViewModel>() };
            var tag = nameof(DailySalesReportAsync);
            _logger.LogInformation($"{tag} get reports: instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.ReceiptLineItems
                    .Include(r => r.Product)
                    .Where(r => r.InstanceId.Equals(instanceId));
                var hasFromDate = DateTime.TryParse(dateFrom, out var dtFrom);
                var hasToDate = DateTime.TryParse(dateTo, out var dtTo);
                if (hasFromDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date >= dtFrom.Date);
                if (hasToDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date <= dtTo.Date);
                if (!string.IsNullOrEmpty(search))
                    dataQuery = dataQuery.Where(r => r.Product.Code.ToLower().Contains(search.ToLower()));
                var data = await dataQuery.OrderByDescending(r => r.DateCreated).ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                var dates = data.Select(a => a.DateCreated.Date).Distinct().ToList();
                foreach (var date in dates)
                {
                    var sales = new DailySalesViewModel
                    {
                        Day = date,
                        TotalSales = data.Where(d => d.DateCreated.Date.Equals(date)).Sum(d => d.Amount),
                        ExpectedProfit = data.Where(d => d.DateCreated.Date.Equals(date)).Sum(d => d.ExpectedProfit),
                        ActualProfit = data.Where(d => d.DateCreated.Date.Equals(date)).Sum(d => d.ActualProfit)
                    };
                    result.Data.Add(sales);
                }
                result.Success = result.Data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                _logger.LogInformation($"{tag} found {result.Data.Count} records");
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

        public async Task<ReturnData<List<MonthSaleViewModel>>> MonthlySalesReportAsync(Guid instanceId, string dateFrom = "", string dateTo = "")
        {
            var result = new ReturnData<List<MonthSaleViewModel>> { Data = new List<MonthSaleViewModel>() };
            var tag = nameof(MonthlySalesReportAsync);
            _logger.LogInformation($"{tag} get reports: instanceId {instanceId}, duration {dateFrom}-{dateTo}");
            try
            {
                var hasDtFrom = DateTime.TryParse(dateFrom, out var dtFrom);
                var hasDtTo = DateTime.TryParse(dateFrom, out var dtTo);
                var months = Helpers.MonthsBetween(hasDtFrom ? dtFrom : DateTime.Now.AddMonths(-6),
                    hasDtTo ? dtTo : DateTime.Now);
                var dataQuery = _context.ReceiptLineItems
                   .Where(i => i.InstanceId.Equals(instanceId));
                foreach (var tuple in months)
                {
                    var firstDay = DateTime.Parse($"01-{tuple.Item1}-{tuple.Item2}");
                    var lastDay = DateTime.Parse($"{tuple.Item3}-{tuple.Item1}-{tuple.Item2}");
                    var totalSales = await dataQuery.Where(d => d.DateCreated.Date >= firstDay.Date
                            && d.DateCreated <= lastDay.Date)
                           .SumAsync(c => c.UnitPrice * c.Quantity);
                    var expectedProfit = await dataQuery.Where(d => d.DateCreated.Date >= firstDay.Date
                            && d.DateCreated <= lastDay.Date)
                           .SumAsync(r => (r.SellingPrice * r.Quantity) - (r.BuyingPrice * r.Quantity));
                    var actualProfit = await dataQuery.Where(d => d.DateCreated.Date >= firstDay.Date
                        && d.DateCreated <= lastDay.Date)
                       .SumAsync(r => (r.UnitPrice * r.Quantity) - (r.BuyingPrice * r.Quantity));
                    var sales = new MonthSaleViewModel
                    {
                        Month = tuple.Item1,
                        TotalSales = totalSales,
                        ExpectedProfit = expectedProfit,
                        ActualProfit = actualProfit
                    };
                    result.Data.Add(sales);
                }
                result.Success = result.Data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                _logger.LogInformation($"{tag} found {result.Data.Count} records");
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
        public async Task<ReturnData<List<CustomerBalanceViewModel>>> CustomerBalancesAsync(Guid instanceId, string dateFrom = "", string dateTo = "")
        {
            var result = new ReturnData<List<CustomerBalanceViewModel>> { Data = new List<CustomerBalanceViewModel>() };
            var tag = nameof(CustomerBalancesAsync);
            _logger.LogInformation($"{tag} get customer balances: instanceId {instanceId}, duration {dateFrom}-{dateTo}");
            try
            {
                var customers = await _context.Customers
                    .Where(c => c.InstanceId.Equals(instanceId))
                    .ToListAsync();
                foreach (var customer in customers)
                {
                    var balance = await _productInterface.GlUserBalanceAsync(GlUserType.Customer, customer.Id);
                    result.Data.Add(new CustomerBalanceViewModel
                    {
                        Customer = customer,
                        Debit = balance.Data.DebitAmount,
                        Credit = balance.Data.CreditAmount
                    });
                }

                result.Success = true;
                result.Message = "Found";
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

        public async Task<ReturnData<CustomerStatementViewModel>> CustomerStatementAsync(Guid customerId, string dateFrom = "", string dateTo = "")
        {
            var result = new ReturnData<CustomerStatementViewModel> { Data = new CustomerStatementViewModel() };
            var tag = nameof(CustomerStatementAsync);
            _logger.LogInformation($"{tag} get customer statement for {customerId}, duration {dateFrom}-{dateTo}");
            try
            {
                var customer = await _context.Customers
                    .Where(c => c.Id.Equals(customerId))
                    .FirstOrDefaultAsync();
                if (customer == null)
                {
                    result.Message = "Customer not found";
                    return result;
                }
                result.Data.Customer = customer;
                var entries = await _context.GeneralLedgerEntries
                    .Where(g => g.UserId.Equals(customerId))
                    .OrderBy(g => g.DateCreated)
                    .ToListAsync();
                if (entries.Any())
                    result.Data.LedgerEntries = entries;
                result.Success = true;
                result.Message = "Found";
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

        public async Task<ReturnData<List<TopSellingProductViewModel>>> SalesByClerkAsync(Guid instanceId, string dateFrom = "", string dateTo = "")
        {
            var result = new ReturnData<List<TopSellingProductViewModel>> { Data = new List<TopSellingProductViewModel>() };
            var tag = nameof(SalesByClerkAsync);
            _logger.LogInformation($"{tag} from {dateFrom} to {dateTo}.  instace id {instanceId}");
            try
            {
                var hasFromDate = DateTime.TryParse(dateFrom, out var dtFrom);
                var hasToDate = DateTime.TryParse(dateTo, out var dtTo);
                var dataQuery = _context.ReceiptLineItems.AsQueryable();
                if (hasFromDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date >= dtFrom.Date);
                if (hasToDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date <= dtTo.Date);

                var dataQry = dataQuery.GroupBy(l => l.Personnel)
                    .Select(tp => new TopSellingProductViewModel
                    {
                        Personnel = tp.Key,
                        Amount = tp.Sum(s => s.UnitPrice * s.Quantity)
                    }).Join(_context.Users,
                    tp => tp.Personnel, u => u.UserName, (tp, u) => new TopSellingProductViewModel
                    {
                        ClerkNames = u.FullName,
                        Personnel = u.UserName,
                        Amount = tp.Amount,
                        Volume = tp.Volume,
                        InstanceId = u.InstanceId,
                        ClientId = u.ClientId
                    }).Where(r => r.InstanceId.Equals(instanceId));
                var data = await dataQry.OrderByDescending(p => p.Amount)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} clerk sales");
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

        public async Task<ReturnData<CloseOfDayViewModel>> CloseOfDayAsync(Guid clientId, Guid? instanceId, DateTime dateFrom, DateTime dateTo, string personnel = "")
        {
            var result = new ReturnData<CloseOfDayViewModel> { Data = new CloseOfDayViewModel() };
            var tag = nameof(CloseOfDayAsync);
            try
            {
                var from = dateFrom.ToString("dd-MMM-yyyy");
                var to = dateTo.ToString("dd-MMM-yyyy");
                _logger.LogInformation($"{tag} on client {clientId} instance {instanceId}  from {from} to {to}");
                var model = new CloseOfDayViewModel()
                {
                    Day = from.Equals(to) ? $"{from}" : $"{from} to {to}"
                };
                var receiptsQuery = _context.Receipts.Where(r => r.ClientId.Equals(clientId))
                    .Include(r => r.ReceiptLineItems).AsQueryable();
                if (instanceId.HasValue)
                    receiptsQuery = receiptsQuery.Where(r => r.InstanceId.Equals(instanceId.Value));
                if (!string.IsNullOrEmpty(personnel))
                {
                    personnel = personnel.ToLower();
                    receiptsQuery = receiptsQuery
                        .Where(r => r.Personnel.ToLower().Equals(personnel));
                }
                var creditSales = await receiptsQuery.Where(r => r.IsCredit)
                    .Where(r => r.DateCreated.Date >= dateFrom.Date && r.DateCreated.Date <= dateTo.Date)
                    .SelectMany(s => s.ReceiptLineItems)
                    .SumAsync(r => r.UnitPrice * r.Quantity);
                var dataQuery = receiptsQuery
                    .SelectMany(s => s.ReceiptLineItems)
                    .AsQueryable();
                var totalSales = await dataQuery
                    .Where(r => r.DateCreated.Date >= dateFrom.Date && r.DateCreated.Date <= dateTo.Date)
                     .SumAsync(r => r.UnitPrice * r.Quantity);
                model.CreditSale = creditSales;
                model.TotalSale = totalSales;
                var salesByClerk = await dataQuery
                    .Where(r => r.DateCreated.Date >= dateFrom.Date && r.DateCreated.Date <= dateTo.Date)
                     .GroupBy(l => l.Personnel)
                    .Select(tp => new KeyAmountViewModel
                    {
                        Key = tp.Key,
                        Amount = tp.Sum(s => s.Quantity * s.UnitPrice)
                    }).Join(_context.Users.Select(u => new { u.UserName, u.FullName }),
                  p => p.Key, u => u.UserName, (p, u) => new KeyAmountViewModel
                  {
                      Amount = p.Amount,
                      Key = u.FullName
                  }).ToListAsync();
                model.SalesByClerk = salesByClerk;
                var invoicesQry = receiptsQuery.Where(r => r.IsCredit
                  && (r.DateLastModified.HasValue
                  && r.DateLastModified.Value.Date >= dateFrom.Date && r.DateLastModified.Value.Date <= dateTo.Date))
                    .Join(_context.GeneralLedgerEntries
                    .Where(l => l.DateCreated.Date >= dateFrom.Date && l.DateCreated.Date <= dateTo.Date)
                    , r => r.Id, gl => gl.DocumentId, (r, gl) => new
                    {
                        r.LastModifiedBy,
                        gl.Personnel,
                        AmountReceived = gl.Credit,
                        r.CustomerId
                    }).AsQueryable();
                model.InvoiceCustomerServed = await invoicesQry
                    .Select(i => i.CustomerId).Distinct()
                    .CountAsync();
                model.TotalRepayment = await invoicesQry.SumAsync(i => i.AmountReceived);
                var receiptsByClerk = await invoicesQry
                    .GroupBy(l => l.LastModifiedBy)
                  .Select(tp => new KeyAmountViewModel
                  {
                      Key = tp.Key,
                      Amount = tp.Sum(s => s.AmountReceived)
                  }).Join(_context.Users.Select(u => new { u.UserName, u.FullName }),
                  p => p.Key, u => u.UserName, (p, u) => new KeyAmountViewModel
                  {
                      Amount = p.Amount,
                      Key = u.FullName
                  }).ToListAsync();
                model.ReceiptsByClerk = receiptsByClerk;

                var paymentsByModesQry = _context.GeneralLedgerEntries
                    .Where(e => e.ClientId.Equals(clientId));
                if (instanceId.HasValue)
                    paymentsByModesQry = paymentsByModesQry.Where(e => e.InstanceId.Equals(instanceId.Value));
                var paymentsByModes = await paymentsByModesQry
                    .Where(g => g.Document.Equals(Document.Receipt) &&
                    g.DateCreated.Date >= dateFrom.Date && g.DateCreated.Date <= dateTo.Date)
                    .Join(_context.Receipts.Include(r => r.PaymentMode), g => g.DocumentId, r => r.Id, (g, r) =>
                            new { r.PaymentMode.Name, g.Credit }
                    ).GroupBy(c => c.Name)
                    .Select(s => new KeyAmountViewModel
                    {
                        Key = s.Key,
                        Amount = s.Sum(a => a.Credit)
                    }).ToListAsync();
                model.PaymentsByMode = paymentsByModes;

                var expensesQry = _context.Expenses.Where(e => e.ClientId.Equals(clientId));
                if (instanceId.HasValue)
                    expensesQry = expensesQry.Where(e => e.InstanceId.Equals(instanceId.Value));
                var expenses = await expensesQry
                    .Include(e => e.ExpenseType).DefaultIfEmpty()
                   .Where(g => g.DateCreated.Date >= dateFrom.Date && g.DateCreated.Date <= dateTo.Date)
                    .Select(r => new { r.ExpenseType.Name, r.Amount }
                   ).GroupBy(c => c.Name)
                   .Select(s => new KeyAmountViewModel
                   {
                       Key = s.Key,
                       Amount = s.Sum(a => a.Amount)
                   }).ToListAsync();
                model.Expenses = expenses;
                result.Success = true;
                result.Message = "Found";
                result.Data = model;
                _logger.LogInformation($"{tag} total sales {model.TotalSale} cash returns {model.DailyCashReturn} expenses {model.TotalExpenses}");
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
