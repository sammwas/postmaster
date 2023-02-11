using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
    public interface IInvoiceInterface
    {
        Task<ReturnData<List<Invoice>>> GetAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "");
        Task<ReturnData<Invoice>> ByIdAsync(Guid id);
        Task<ReturnData<Invoice>> PayAsync(Guid id, string personnel);
    }

    public class InvoiceImplementation : IInvoiceInterface
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<InvoiceImplementation> _logger;
        public InvoiceImplementation(DatabaseContext context, ILogger<InvoiceImplementation> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReturnData<List<Invoice>>> ByClientIdAsync(Guid clientId)
        {
            var result = new ReturnData<List<Invoice>> { Data = new List<Invoice>() };
            var tag = nameof(ByClientIdAsync);
            _logger.LogInformation($"{tag} get all client {clientId} invoices");
            try
            {
                var data = await _context.Invoices
                    .Include(c => c.Receipt)
                    .ThenInclude(c => c.ReceiptLineItems)
                    .Where(c => c.ClientId.Equals(clientId))
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} invoices");
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

        public async Task<ReturnData<Invoice>> ByIdAsync(Guid id)
        {
            var result = new ReturnData<Invoice> { Data = new Invoice() };
            var tag = nameof(ByIdAsync);
            _logger.LogInformation($"{tag} get invoice by id {id}");
            try
            {
                var invoice = await _context.Invoices
                    .Include(p => p.Receipt)
                    .ThenInclude(p => p.Customer)
                    .Include(r => r.Receipt)
                    .ThenInclude(r => r.ReceiptLineItems)
                    .FirstOrDefaultAsync(c => c.Id.Equals(id));
                if (invoice != null)
                {
                    result.Success = true;
                    invoice.Receipt.ReceiptLineItems.ForEach(l =>
                    {
                        l.Product = _context.Products.FirstOrDefault(p => p.Id.Equals(l.ProductId));
                    });
                }
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = invoice;
                _logger.LogInformation($"{tag} invoice {id} {result.Message}");
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

        public async Task<ReturnData<List<Invoice>>> GetAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "")
        {
            var result = new ReturnData<List<Invoice>> { Data = new List<Invoice>() };
            var tag = nameof(GetAsync);
            _logger.LogInformation($"{tag} get invoices: clientId {clientId}, instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.Invoices
                    .Include(c => c.Receipt)
                    .ThenInclude(u => u.Customer)
                    .Include(c => c.Receipt).ThenInclude(r => r.ReceiptLineItems)
                    .AsQueryable();
                if (clientId != null)
                    dataQuery = dataQuery.Where(r => r.ClientId.Equals(clientId.Value));
                if (instanceId != null)
                    dataQuery = dataQuery.Where(r => r.InstanceId.Equals(instanceId.Value));
                var hasFromDate = DateTime.TryParse(dateFrom, out var dtFrom);
                var hasToDate = DateTime.TryParse(dateTo, out var dtTo);
                if (hasFromDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date >= dtFrom.Date);
                if (hasToDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date <= dtTo.Date);
                if (!string.IsNullOrEmpty(search))
                    dataQuery = dataQuery.Where(r => r.Code.ToLower().Contains(search.ToLower()));
                var data = await dataQuery.OrderByDescending(r => r.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} invoices");
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

        public async Task<ReturnData<Invoice>> PayAsync(Guid id, string personnel)
        {
            var result = new ReturnData<Invoice> { Data = new Invoice() };
            var tag = nameof(PayAsync);
            _logger.LogInformation($"{tag} mark invoice {id} as paid by {personnel}");
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Receipt)
                    .ThenInclude(r => r.ReceiptLineItems)
                    .FirstOrDefaultAsync(i => i.Id.Equals(id));
                if (invoice == null)
                {
                    result.Message = "Provided invoice not Found";
                    _logger.LogInformation($"{tag} unable to update invoice details : {result.Message}");
                    return result;
                }
                invoice.Receipt.DateLastModified = DateTime.Now;
                invoice.Receipt.LastModifiedBy = personnel;
                invoice.Receipt.AmountReceived = invoice.Receipt.Amount;
                invoice.Status = EntityStatus.Closed;
                invoice.LastModifiedBy = personnel;
                invoice.DateLastModified = DateTime.Now;
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = result.Success ? "Found" : "Not Found";
                result.Data = invoice;
                _logger.LogInformation($"{tag} invoice {id} {invoice.Code} payment updated for receipt {invoice.Receipt.Code}");
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
