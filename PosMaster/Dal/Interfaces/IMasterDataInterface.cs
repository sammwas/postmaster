using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
    public interface IMasterDataInterface
    {
        Task<ReturnData<List<ProductCategory>>> ProductCategoriesAsync(Guid clientId);
        Task<ReturnData<ProductCategory>> EditProductCategoryAsync(ProductCategoryViewModel model);
        Task<ReturnData<ProductCategory>> ByIdProductCategoryAsync(Guid id);
        Task<ReturnData<List<UnitOfMeasure>>> UnitOfMeasuresAsync(Guid clientId);
        Task<ReturnData<UnitOfMeasure>> EditUnitOfMeasureAsync(UnitOfMeasureViewModel model);
        Task<ReturnData<UnitOfMeasure>> ByIdUnitOfMeasureAsync(Guid id);
        Task<ReturnData<List<ExpenseType>>> ExpenseTypesAsync(Guid clientId);
        Task<ReturnData<ExpenseType>> EditExpenseTypeAsync(ExpenseTypeViewModel model);
        Task<ReturnData<ExpenseType>> ByIdExpenseTypeAsync(Guid id);
        Task<ReturnData<List<PaymentMode>>> PaymentModesAsync(Guid clientId);
        Task<ReturnData<PaymentMode>> EditPaymentModeAsync(PaymentModeViewModel model);
        Task<ReturnData<PaymentMode>> ByIdPaymentModeAsync(Guid id);
        Task<ReturnData<List<TaxType>>> TaxTypesAsync(Guid clientId);
        Task<ReturnData<TaxType>> EditTaxTypesAsync(TaxTypeViewModel model);
        Task<ReturnData<TaxType>> ByIdTaxTypeAsync(Guid id);
    }

    public class MasterDataImplementation : IMasterDataInterface
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<MasterDataImplementation> _logger;
        public MasterDataImplementation(DatabaseContext context, ILogger<MasterDataImplementation> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ReturnData<ExpenseType>> ByIdExpenseTypeAsync(Guid id)
        {
            var result = new ReturnData<ExpenseType> { Data = new ExpenseType() };
            var tag = nameof(ByIdExpenseTypeAsync);
            _logger.LogInformation($"{tag} get expense type by id {id}");
            try
            {
                var data = await _context.ExpenseTypes.FirstOrDefaultAsync(e => e.Id.Equals(id));
                result.Success = data != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} expense type {id} {result.Message}");
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

        public async Task<ReturnData<PaymentMode>> ByIdPaymentModeAsync(Guid id)
        {
            var result = new ReturnData<PaymentMode> { Data = new PaymentMode() };
            var tag = nameof(ByIdExpenseTypeAsync);
            _logger.LogInformation($"{tag} get payment mode by id {id}");
            try
            {
                var data = await _context.PaymentModes.FirstOrDefaultAsync(e => e.Id.Equals(id));
                result.Success = data != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} payment mode {id} {result.Message}");
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

        public async Task<ReturnData<ProductCategory>> ByIdProductCategoryAsync(Guid id)
        {
            var result = new ReturnData<ProductCategory> { Data = new ProductCategory() };
            var tag = nameof(ByIdProductCategoryAsync);
            _logger.LogInformation($"{tag} get product category by id {id}");
            try
            {
                var data = await _context.ProductCategories.FirstOrDefaultAsync(e => e.Id.Equals(id));
                result.Success = data != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} product category {id} {result.Message}");
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

        public async Task<ReturnData<TaxType>> ByIdTaxTypeAsync(Guid id)
        {
            var result = new ReturnData<TaxType> { Data = new TaxType() };
            var tag = nameof(ByIdTaxTypeAsync);
            _logger.LogInformation($"{tag} get TaxType by id {id}");
            try
            {
                var data = await _context.TaxTypes.FirstOrDefaultAsync(e => e.Id.Equals(id));
                result.Success = data != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} TaxType {id} {result.Message}");
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

        public async Task<ReturnData<UnitOfMeasure>> ByIdUnitOfMeasureAsync(Guid id)
        {
            var result = new ReturnData<UnitOfMeasure> { Data = new UnitOfMeasure() };
            var tag = nameof(ByIdProductCategoryAsync);
            _logger.LogInformation($"{tag} get UnitOfMeasure by id {id}");
            try
            {
                var data = await _context.UnitOfMeasures.FirstOrDefaultAsync(e => e.Id.Equals(id));
                result.Success = data != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} UnitOfMeasure {id} {result.Message}");
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

        public async Task<ReturnData<ExpenseType>> EditExpenseTypeAsync(ExpenseTypeViewModel model)
        {
            var result = new ReturnData<ExpenseType> { Data = new ExpenseType() };
            var tag = nameof(EditExpenseTypeAsync);
            _logger.LogInformation($"{tag} edit expense type");
            try
            {
                if (model.IsEditMode)
                {
                    var dbExpenseType = await _context.ExpenseTypes
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbExpenseType == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbExpenseType.Code = model.Code;
                    dbExpenseType.Name = model.Name;
                    dbExpenseType.LastModifiedBy = model.Personnel;
                    dbExpenseType.DateLastModified = DateTime.Now;
                    dbExpenseType.Notes = model.Notes;
                    dbExpenseType.Status = model.Status;
                    dbExpenseType.MaxApprovedAmount = model.MaxApprovedAmount;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbExpenseType;
                    _logger.LogInformation($"{tag} updated {dbExpenseType.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var expenseType = new ExpenseType
                {
                    Code = model.Code,
                    Name = model.Name,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    MaxApprovedAmount = model.MaxApprovedAmount
                };
                _context.ExpenseTypes.Add(expenseType);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = expenseType;
                _logger.LogInformation($"{tag} added {expenseType.Name}  {expenseType.Id} : {result.Message}");
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

        public async Task<ReturnData<PaymentMode>> EditPaymentModeAsync(PaymentModeViewModel model)
        {
            var result = new ReturnData<PaymentMode> { Data = new PaymentMode() };
            var tag = nameof(EditPaymentModeAsync);
            _logger.LogInformation($"{tag} edit payment mode");
            try
            {
                if (model.IsEditMode)
                {
                    var dbMode = await _context.PaymentModes
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbMode == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbMode.Code = model.Code;
                    dbMode.Name = model.Name;
                    dbMode.LastModifiedBy = model.Personnel;
                    dbMode.DateLastModified = DateTime.Now;
                    dbMode.Notes = model.Notes;
                    dbMode.Status = model.Status;
                    if (model.IsNewImage)
                        dbMode.ImagePath = model.ImagePath;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbMode;
                    _logger.LogInformation($"{tag} updated {dbMode.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var mode = new PaymentMode
                {
                    Code = model.Code,
                    Name = model.Name,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    ImagePath = model.ImagePath
                };
                _context.PaymentModes.Add(mode);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = mode;
                _logger.LogInformation($"{tag} added {mode.Name}  {mode.Id} : {result.Message}");
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

        public async Task<ReturnData<ProductCategory>> EditProductCategoryAsync(ProductCategoryViewModel model)
        {
            var result = new ReturnData<ProductCategory> { Data = new ProductCategory() };
            var tag = nameof(EditProductCategoryAsync);
            _logger.LogInformation($"{tag} edit product category");
            try
            {
                if (model.IsEditMode)
                {
                    var dbCategory = await _context.ProductCategories
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbCategory == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbCategory.Code = model.Code;
                    dbCategory.Name = model.Name;
                    dbCategory.LastModifiedBy = model.Personnel;
                    dbCategory.DateLastModified = DateTime.Now;
                    dbCategory.Notes = model.Notes;
                    dbCategory.Status = model.Status;
                    if (model.IsNewImage)
                        dbCategory.ImagePath = model.ImagePath;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbCategory;
                    _logger.LogInformation($"{tag} updated {dbCategory.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var category = new ProductCategory
                {
                    Code = model.Code,
                    Name = model.Name,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    ImagePath = model.ImagePath
                };
                _context.ProductCategories.Add(category);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = category;
                _logger.LogInformation($"{tag} added {category.Name}  {category.Id} : {result.Message}");
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

        public async Task<ReturnData<TaxType>> EditTaxTypesAsync(TaxTypeViewModel model)
        {
            var result = new ReturnData<TaxType> { Data = new TaxType() };
            var tag = nameof(EditTaxTypesAsync);
            _logger.LogInformation($"{tag} edit tax type");
            try
            {
                if (model.IsEditMode)
                {
                    var dbUnit = await _context.TaxTypes
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbUnit == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbUnit.Code = model.Code;
                    dbUnit.Name = model.Name;
                    dbUnit.LastModifiedBy = model.Personnel;
                    dbUnit.DateLastModified = DateTime.Now;
                    dbUnit.Notes = model.Notes;
                    dbUnit.Status = model.Status;
                    dbUnit.Rate = model.Rate;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbUnit;
                    _logger.LogInformation($"{tag} updated {dbUnit.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var unit = new TaxType
                {
                    Code = model.Code,
                    Name = model.Name,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    Rate = model.Rate
                };
                _context.TaxTypes.Add(unit);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = unit;
                _logger.LogInformation($"{tag} added {unit.Name}  {unit.Id} : {result.Message}");
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

        public async Task<ReturnData<UnitOfMeasure>> EditUnitOfMeasureAsync(UnitOfMeasureViewModel model)
        {
            var result = new ReturnData<UnitOfMeasure> { Data = new UnitOfMeasure() };
            var tag = nameof(EditUnitOfMeasureAsync);
            _logger.LogInformation($"{tag} edit unit of measure");
            try
            {
                if (model.IsEditMode)
                {
                    var dbUnit = await _context.UnitOfMeasures
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbUnit == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbUnit.Code = model.Code;
                    dbUnit.Name = model.Name;
                    dbUnit.LastModifiedBy = model.Personnel;
                    dbUnit.DateLastModified = DateTime.Now;
                    dbUnit.Notes = model.Notes;
                    dbUnit.Status = model.Status;
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbUnit;
                    _logger.LogInformation($"{tag} updated {dbUnit.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var unit = new UnitOfMeasure
                {
                    Code = model.Code,
                    Name = model.Name,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Status = model.Status
                };
                _context.UnitOfMeasures.Add(unit);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = unit;
                _logger.LogInformation($"{tag} added {unit.Name}  {unit.Id} : {result.Message}");
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

        public async Task<ReturnData<List<ExpenseType>>> ExpenseTypesAsync(Guid clientId)
        {
            var result = new ReturnData<List<ExpenseType>> { Data = new List<ExpenseType>() };
            var tag = nameof(ExpenseTypesAsync);
            _logger.LogInformation($"{tag} get all client {clientId} expense types");
            try
            {
                var data = await _context.ExpenseTypes
                    .Where(c => c.ClientId.Equals(clientId))
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} expense types");
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

        public async Task<ReturnData<List<PaymentMode>>> PaymentModesAsync(Guid clientId)
        {
            var result = new ReturnData<List<PaymentMode>> { Data = new List<PaymentMode>() };
            var tag = nameof(PaymentModesAsync);
            _logger.LogInformation($"{tag} get all client {clientId} payment modes");
            try
            {
                var data = await _context.PaymentModes
                    .Where(c => c.ClientId.Equals(clientId))
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} payment modes");
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

        public async Task<ReturnData<List<ProductCategory>>> ProductCategoriesAsync(Guid clientId)
        {
            var result = new ReturnData<List<ProductCategory>> { Data = new List<ProductCategory>() };
            var tag = nameof(ProductCategoriesAsync);
            _logger.LogInformation($"{tag} get all client {clientId} product categories");
            try
            {
                var data = await _context.ProductCategories
                    .Where(c => c.ClientId.Equals(clientId))
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} product categories");
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

        public async Task<ReturnData<List<TaxType>>> TaxTypesAsync(Guid clientId)
        {
            var result = new ReturnData<List<TaxType>> { Data = new List<TaxType>() };
            var tag = nameof(TaxTypesAsync);
            _logger.LogInformation($"{tag} get all client {clientId} tax types");
            try
            {
                var data = await _context.TaxTypes
                    .Where(c => c.ClientId.Equals(clientId))
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} tax types");
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

        public async Task<ReturnData<List<UnitOfMeasure>>> UnitOfMeasuresAsync(Guid clientId)
        {
            var result = new ReturnData<List<UnitOfMeasure>> { Data = new List<UnitOfMeasure>() };
            var tag = nameof(UnitOfMeasuresAsync);
            _logger.LogInformation($"{tag} get all client {clientId} unit of measures");
            try
            {
                var data = await _context.UnitOfMeasures
                    .Where(c => c.ClientId.Equals(clientId))
                    .OrderByDescending(c => c.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} unit of measures");
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
