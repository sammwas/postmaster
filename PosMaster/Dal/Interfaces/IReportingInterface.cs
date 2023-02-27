﻿using Microsoft.EntityFrameworkCore;
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
                var months = Helpers.MonthsBetween(hasDtFrom ? dtFrom : DateTime.Parse($"01-01-{DateTime.Now.Year}"),
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
    }
}
