using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
    public interface IExpenseInterface
    {
        Task<ReturnData<List<Expense>>> AllAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "", string personnel = "");
        Task<ReturnData<Expense>> EditAsync(ExpenseViewModel model);
        Task<ReturnData<Expense>> ByIdAsync(Guid id);
    }

    public class ExpenseImplementation : IExpenseInterface
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<ExpenseImplementation> _logger;
        public ExpenseImplementation(DatabaseContext context, ILogger<ExpenseImplementation> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReturnData<List<Expense>>> AllAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "", string personnel = "")
        {
            var result = new ReturnData<List<Expense>> { Data = new List<Expense>() };
            var tag = nameof(AllAsync);
            _logger.LogInformation($"{tag} get expenses : clientId {clientId}, instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.Expenses.Include(r => r.ExpenseType)
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
                if (!string.IsNullOrEmpty(personnel))
                    dataQuery = dataQuery.Where(r => r.Personnel.Equals(personnel));

                var data = await dataQuery.OrderByDescending(r => r.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} receipts");
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

        public async Task<ReturnData<Expense>> EditAsync(ExpenseViewModel model)
        {
            var result = new ReturnData<Expense> { Data = new Expense() };
            var tag = nameof(EditAsync);
            _logger.LogInformation($"{tag} edit expense");
            try
            {
                var expenseType = await _context.ExpenseTypes
                    .FirstOrDefaultAsync(t => t.Id.Equals(Guid.Parse(model.ExpenseTypeId)));
                if (expenseType == null)
                {
                    result.Message = "Provided expense type not Found";
                    _logger.LogWarning($"{tag} edit failed {model.Id} : {result.Message}");
                    return result;
                }
                if (model.IsEditMode)
                {
                    var dbExpense = await _context.Expenses
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbExpense == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogWarning($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbExpense.Code = model.Code;
                    dbExpense.ExpenseTypeId = expenseType.Id;
                    dbExpense.Amount = model.Amount;
                    dbExpense.LastModifiedBy = model.Personnel;
                    dbExpense.DateLastModified = DateTime.Now;
                    dbExpense.Notes = model.Notes;
                    dbExpense.Status = model.Status;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbExpense;
                    _logger.LogInformation($"{tag} updated {expenseType.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var nextCount = _context.Expenses.Count(c => c.ClientId.Equals(model.ClientId)) + 1;
                var expense = new Expense
                {
                    Code = $"PV-{expenseType.Code}-{nextCount}",
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    Amount = model.Amount,
                    ExpenseTypeId = Guid.Parse(model.ExpenseTypeId)
                };
                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = expense;
                _logger.LogInformation($"{tag} added {expenseType.Name} {expense.Code}  {expense.Id} : {result.Message}");
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
        public async Task<ReturnData<Expense>> ByIdAsync(Guid id)
        {
            var result = new ReturnData<Expense> { Data = new Expense() };
            var tag = nameof(ByIdAsync);
            _logger.LogInformation($"{tag} get expense by id {id}");
            try
            {
                var expense = await _context.Expenses.Include(p => p.ExpenseType)
                    .FirstOrDefaultAsync(c => c.Id.Equals(id));
                result.Success = expense != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = expense;
                _logger.LogInformation($"{tag} expense {id} {result.Message}");
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
