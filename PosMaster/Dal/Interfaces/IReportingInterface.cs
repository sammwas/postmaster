using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
    public interface IReportingInterface
    {
        Task<ReturnData<List<DailySalesViewModel>>> DailySalesReportAsync(Guid? clientId, string instanceId = "", string dateFrom = "", string dateTo = "", string search = "");

    }
    public class ReportingImplementation : IReportingInterface
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<ReportingImplementation> _logger;
        public ReportingImplementation(DatabaseContext context, ILogger<ReportingImplementation> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ReturnData<List<DailySalesViewModel>>> DailySalesReportAsync(Guid? clientId, string instanceId = "", string dateFrom = "", string dateTo = "", string search = "")
        {
            var result = new ReturnData<List<DailySalesViewModel>> { Data = new List<DailySalesViewModel>() };
            var tag = nameof(DailySalesReportAsync);
            _logger.LogInformation($"{tag} get reports: instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.ReceiptLineItems
                    .Include(r => r.Product)
                    .AsQueryable();
                if (clientId != null)
                    dataQuery = dataQuery.Where(d => d.ClientId.Equals(clientId.Value));
                if (!string.IsNullOrEmpty(instanceId))
                    dataQuery = dataQuery.Where(r => r.InstanceId.Equals(Guid.Parse(instanceId)));
                var hasFromDate = DateTime.TryParse(dateFrom, out var dtFrom);
                var hasToDate = DateTime.TryParse(dateTo, out var dtTo);
                if (hasFromDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date >= dtFrom.Date);
                if (hasToDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date <= dtTo.Date);
                if (!string.IsNullOrEmpty(search))
                    dataQuery = dataQuery.Where(r => r.Product.Code.ToLower().Contains(search.ToLower()));
                var data = await dataQuery.OrderBy(r => r.DateCreated).ToListAsync();
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

                _logger.LogInformation($"{tag} found {data.Count} records");
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
