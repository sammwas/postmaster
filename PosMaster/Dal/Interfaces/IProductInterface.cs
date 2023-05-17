using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
    public interface IProductInterface
    {
        Task<ReturnData<Product>> EditAsync(ProductViewModel model);
        Task<ReturnData<List<Product>>> ByInstanceIdAsync(Guid clientId, Guid? instanceId = null, bool isPos = false, string search = "");
        Task<ReturnData<Product>> ByIdAsync(Guid id);
        Task<ReturnData<Receipt>> ProductsSaleAsync(ProductSaleViewModel model);
        Task<ReturnData<List<Receipt>>> ReceiptsAsync(Guid clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "",
            string personnel = "", string modeId = "", string type = "");
        Task<ReturnData<ProductStockAdjustmentLog>> AdjustProductStockAsync(ProductStockAdjustmentViewModel model);
        Task<ReturnData<PurchaseOrder>> AddPurchaseOrderAsync(PoViewModel model);
        Task<ReturnData<List<PurchaseOrder>>> PurchaseOrdersAsync(Guid clientId, Guid? instanceId, bool onlyActive = false, string dateFrom = "", string dateTo = "",
            string search = "", string personnel = "");
        Task<ReturnData<List<GoodReceivedNote>>> GoodsReceivedAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "", string personnel = "");
        Task<ReturnData<List<GeneralLedgerEntry>>> GeneralLedgersAsync(Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "");
        Task<ReturnData<List<ProductPoQuantityLog>>> ProductPoQuantityLogAsync(Guid clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "");
        Task<ReturnData<PurchaseOrder>> PurchaseOrderByIdAsync(Guid id);
        string DocumentRefNumber(Document document, Guid clientId);
        Task<ReturnData<List<Product>>> LowStockProductsAsync(Guid? instanceId, int limit = 10);
        Task<ReturnData<List<TopSellingProductViewModel>>> TopSellingProductsByVolumeAsync(Guid? instanceId, string dtFrom = "", string dtTo = "",
           string personnel = "", int limit = 10);
        Task<ReturnData<ProductPriceViewModel>> EditPriceAsync(ProductPriceViewModel model);
        Task<ReturnData<Receipt>> ReceiptByIdAsync(string id);
        Task<ReturnData<PurchaseOrder>> EditPurchaseOrderAsync(PurchaseOrderViewModel model);
        Task<ReturnData<string>> PrintReceiptByIdAsync(Guid id, string personnel);
        Task<ReturnData<GoodReceivedNote>> EditGrnAsync(GoodsReceivedNoteViewModel model);
        Task<ReturnData<GoodReceivedNote>> GrnByIdAsync(Guid id);
        Task<ReturnData<ProductPriceLog>> ProductPriceAsync(ItemPriceViewModel model);
        Task<ReturnData<ProductDataViewModel>> ProductDetailsAsync(string productCode, Guid instanceId);
        Task<ReturnData<string>> ReceiptUserAsync(ReceiptUserViewModel model);
        Task<string> ReceiptExcessAmount(ReceiptUserViewModel model);
        Task<string> AddInvoiceAsync(Receipt receipt);
        Guid DefaultClientProductId(Guid clientId);
        Task<ReturnData<ReceiptUserViewModel>> GlUserBalanceAsync(GlUserType userType, Guid userId);
        Task<ReturnData<string>> CancelReceiptAsync(CancelReceiptViewModel model);
        Task<ReturnData<string>> PaySupplierGrnAsync(ExpenseViewModel model);

    }

    public class ProductImplementation : IProductInterface
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<ProductImplementation> _logger;
        public ProductImplementation(DatabaseContext context, ILogger<ProductImplementation> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReturnData<PurchaseOrder>> AddPurchaseOrderAsync(PoViewModel model)
        {
            var result = new ReturnData<PurchaseOrder> { Data = new PurchaseOrder() };
            var tag = nameof(AddPurchaseOrderAsync);
            _logger.LogInformation($"{tag} create purchase order for instance {model.InstanceId}");
            try
            {
                var lineItems = string.IsNullOrEmpty(model.ProductsItemsListStr) ?
                new List<PoGrnProductViewModel>()
                : JsonConvert.DeserializeObject<List<PoGrnProductViewModel>>(model.ProductsItemsListStr);
                if (!lineItems.Any())
                {
                    result.Message = "No line items found";
                    _logger.LogWarning($"{tag} order failed  {model.InstanceId} : {result.Message}");
                    return result;
                }

                var poRef = DocumentRefNumber(Document.Po, model.ClientId);
                var purchaseOrder = new PurchaseOrder
                {
                    Id = Guid.NewGuid(),
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Code = poRef,
                    Name = model.Name,
                    Notes = model.Notes,
                    SupplierId = Guid.Parse(model.SupplierId),
                    Personnel = model.Personnel
                };
                _context.PurchaseOrders.Add(purchaseOrder);
                foreach (var item in lineItems)
                {
                    var lineProduct = new PoGrnProduct
                    {
                        PurchaseOrderId = purchaseOrder.Id,
                        ProductId = item.ProductId,
                        PoNotes = item.Notes,
                        PoQuantity = item.Quantity,
                        PoUnitPrice = item.UnitPrice,
                        Personnel = model.Personnel,
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId,
                        Code = $"{poRef}_{item.ProductName}"
                    };
                    _context.PoGrnProducts.Add(lineProduct);
                }
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Data = purchaseOrder;
                result.Message = $"PO {poRef} Added";
                _logger.LogInformation($"{tag} added {lineItems.Count} products: {result.Message}");
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

        public async Task<ReturnData<ProductStockAdjustmentLog>> AdjustProductStockAsync(ProductStockAdjustmentViewModel model)
        {
            var result = new ReturnData<ProductStockAdjustmentLog> { Data = new ProductStockAdjustmentLog() };
            var tag = nameof(AdjustProductStockAsync);
            _logger.LogInformation($"{tag} adjust stock for product {model.ProductId}");
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id.Equals(Guid.Parse(model.ProductId)));
                if (product == null)
                {
                    result.Message = "Not Found";
                    _logger.LogWarning($"{tag} adjustment failed {model.ProductId} : {result.Message}");
                    return result;
                }
                if (!product.AvailableQuantity.Equals(model.QuantityTo))
                {
                    var log = new ProductStockAdjustmentLog
                    {
                        Code = $"{product.Code}-{DateTime.Now}",
                        ProductId = product.Id,
                        QuantityFrom = product.AvailableQuantity,
                        QuantityTo = model.QuantityTo,
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId,
                        Personnel = model.Personnel,
                        Notes = model.Notes
                    };
                    _context.ProductStockAdjustmentLogs.Add(log);
                }
                var lastPoQty = await _context.ProductPoQuantityLogs
                    .Where(p => p.ProductId.Equals(product.Id))
                    .OrderByDescending(p => p.DateCreated)
                    .FirstOrDefaultAsync();
                if (lastPoQty != null)
                {
                    lastPoQty.AvailableQuantity = model.QuantityTo;
                    lastPoQty.BuyingPrice = model.BuyingPriceTo;
                    lastPoQty.LastModifiedBy = model.Personnel;
                    lastPoQty.DateLastModified = DateTime.Now;
                }
                else
                {
                    var productPo = new ProductPoQuantityLog
                    {
                        ClientId = product.ClientId,
                        InstanceId = product.InstanceId,
                        ProductId = product.Id,
                        Personnel = model.Personnel,
                        AvailableQuantity = model.QuantityTo,
                        DeliveredQuantity = model.QuantityTo - product.AvailableQuantity,
                        BuyingPrice = model.BuyingPriceTo,
                        Notes = model.Notes,
                        Code = $"{product.Code}-{DateTime.Now}"
                    };
                    _context.ProductPoQuantityLogs.Add(productPo);
                }

                product.AvailableQuantity = model.QuantityTo;
                product.BuyingPrice = model.BuyingPriceTo;
                product.LastModifiedBy = model.Personnel;
                product.DateLastModified = DateTime.Now;
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Adjusted";
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

        public async Task<ReturnData<Product>> ByIdAsync(Guid id)
        {
            var result = new ReturnData<Product> { Data = new Product() };
            var tag = nameof(ByIdAsync);
            _logger.LogInformation($"{tag} get product by id {id}");
            try
            {
                var client = await _context.Products.Include(p => p.ProductCategory)
                    .FirstOrDefaultAsync(c => c.Id.Equals(id));
                result.Success = client != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = client;
                _logger.LogInformation($"{tag} product {id} {result.Message}");
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

        public async Task<ReturnData<List<Product>>> ByInstanceIdAsync(Guid clientId, Guid? instanceId, bool isPos = false, string search = "")
        {
            var result = new ReturnData<List<Product>> { Data = new List<Product>() };
            var tag = nameof(ByInstanceIdAsync);
            _logger.LogInformation($"{tag} client {clientId} get all instance {instanceId} products, POS {isPos} , search {search} ");
            try
            {
                var dataQry = _context.Products
                    .Include(c => c.ProductCategory)
                    .Include(c => c.UnitOfMeasure)
                    .Include(p => p.TaxType)
                    .Where(c => c.ClientId.Equals(clientId))
                    .Where(p => !p.Code.Equals(Constants.DefaultProductCode))
                    .AsQueryable();
                if (instanceId != null)
                    dataQry = dataQry.Where(d => d.InstanceId.Equals(instanceId.Value));
                if (isPos)
                {
                    dataQry = dataQry.Where(d => d.Status.Equals(EntityStatus.Active));
                    dataQry = dataQry.Where(d => d.IsService || (d.AvailableQuantity > 0 && d.SellingPrice > 0))
                    .Where(d => d.AvailableQuantity > 0 && d.SellingPrice > 0 && d.PriceStartDate.Date <= DateTime.Now.Date && (d.PriceEndDate == null
                    || d.PriceEndDate.Value.Date >= DateTime.Now.Date));
                }
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    dataQry = dataQry.Where(d => d.Code.ToLower().Contains(search)
                    || d.Name.ToLower().Contains(search))
                    .OrderBy(d => d.Code)
                    .Take(10);
                }
                var data = await dataQry.ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} products");
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

        public async Task<ReturnData<Product>> EditAsync(ProductViewModel model)
        {
            var result = new ReturnData<Product> { Data = new Product() };
            var tag = nameof(EditAsync);
            _logger.LogInformation($"{tag} edit product {model.Code}");
            try
            {
                var hasTaxTypeId = Guid.TryParse(model.TaxTypeId, out var taxTypeId);
                var hasUom = Guid.TryParse(model.UnitOfMeasureId, out var uomId);
                var hasStartDate = DateTime.TryParse(model.PriceStartDateStr, out var startDate);
                var hasEndDate = DateTime.TryParse(model.PriceEndDateStr, out var endDate);
                var dbProduct = model.IsExcelUpload ? await _context.Products
                        .FirstOrDefaultAsync(c => c.Code.ToLower().Equals(model.Code.ToLower()))
                : await _context.Products
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                if (dbProduct != null)
                {
                    dbProduct.Code = model.Code;
                    dbProduct.ProductCategoryId = Guid.Parse(model.ProductCategoryId);
                    dbProduct.Name = model.Name;
                    dbProduct.ReorderLevel = model.ReorderLevel;
                    dbProduct.AllowDiscount = model.AllowDiscount;
                    dbProduct.LastModifiedBy = model.Personnel;
                    dbProduct.DateLastModified = DateTime.Now;
                    dbProduct.Notes = model.Notes;
                    dbProduct.Status = model.Status;
                    dbProduct.IsService = model.IsService;
                    dbProduct.TaxTypeId = hasTaxTypeId ? taxTypeId : (Guid?)null;
                    dbProduct.UnitOfMeasureId = hasUom ? uomId : (Guid?)null;
                    if (model.IsExcelUpload)
                    {
                        dbProduct.BuyingPrice = model.BuyingPrice;
                        dbProduct.AvailableQuantity = model.AvailableQuantity;
                        dbProduct.SellingPrice = model.SellingPrice;
                        dbProduct.PriceStartDate = hasStartDate ? startDate : DateTime.Now;
                        dbProduct.PriceEndDate = hasEndDate ? endDate : (DateTime?)null;
                    }
                    if (model.IsNewImage)
                        dbProduct.ImagePath = model.ImagePath;

                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbProduct;
                    _logger.LogInformation($"{tag} updated {dbProduct.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var stamp = $"{model.InstanceId}_{model.Code}";
                if (await _context.Products.AnyAsync(p => p.ProductInstanceStamp.Equals(stamp)))
                {
                    result.Message = "Exists on instance";
                    _logger.LogInformation($"{tag} added {model.Name} - {model.Code} : {result.Message}");
                    return result;
                }

                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Code = model.Code,
                    ProductCategoryId = Guid.Parse(model.ProductCategoryId),
                    Name = model.Name,
                    ReorderLevel = model.ReorderLevel,
                    BuyingPrice = model.BuyingPrice,
                    SellingPrice = model.SellingPrice,
                    AllowDiscount = model.AllowDiscount,
                    AvailableQuantity = model.AvailableQuantity,
                    UnitOfMeasureId = hasUom ? uomId : (Guid?)null,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    ImagePath = model.ImagePath,
                    TaxTypeId = hasTaxTypeId ? taxTypeId : (Guid?)null,
                    PriceStartDate = hasStartDate ? startDate : DateTime.Now,
                    PriceEndDate = hasEndDate ? endDate : (DateTime?)null,
                    ProductInstanceStamp = stamp
                };
                var productPo = new ProductPoQuantityLog
                {
                    ClientId = product.ClientId,
                    InstanceId = product.InstanceId,
                    ProductId = product.Id,
                    Personnel = model.Personnel,
                    AvailableQuantity = model.AvailableQuantity,
                    BuyingPrice = model.BuyingPrice,
                    Notes = model.Notes,
                    Code = $"{product.Code}-{DateTime.Now}"
                };
                _context.Products.Add(product);
                _context.ProductPoQuantityLogs.Add(productPo);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = product;
                _logger.LogInformation($"{tag} added {product.Name}  {product.Id} : {result.Message}");
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

        public async Task<ReturnData<Receipt>> ProductsSaleAsync(ProductSaleViewModel model)
        {
            var result = new ReturnData<Receipt> { Data = new Receipt() };
            var tag = nameof(ProductsSaleAsync);
            _logger.LogInformation($"{tag} sell : credit {model.IsCredit} , walkin {model.IsWalkIn}");
            try
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Id.Equals(Guid.Parse(model.CustomerId)));
                var lineItems = string.IsNullOrEmpty(model.LineItemListStr) ?
                    new List<ReceiptLineItemMiniViewModel>()
                    : JsonConvert.DeserializeObject<List<ReceiptLineItemMiniViewModel>>(model.LineItemListStr);

                if (customer == null)
                {
                    result.Message = "Provided customer not Found";
                    _logger.LogWarning($"{tag} sale failed {model.CustomerId} : {result.Message}");
                    return result;
                }
                model.IsWalkIn = customer.Code.Equals(Constants.WalkInCustomerCode);
                if (model.IsCredit && model.IsWalkIn)
                {
                    result.Message = $"Credit Sale for {Constants.WalkInCustomerCode} not Allowed";
                    _logger.LogWarning($"{tag} sale failed {model.CustomerId} : {result.Message}");
                    return result;
                }
                if (!lineItems.Any())
                {
                    result.Message = "No line items found";
                    _logger.LogWarning($"{tag} sale failed {model.CustomerId} : {result.Message}");
                    return result;
                }

                var rcptRef = DocumentRefNumber(Document.Receipt, model.ClientId);
                var hasModeId = Guid.TryParse(model.PaymentModeIdStr, out var payModeId);
                var receipt = new Receipt
                {
                    Id = Guid.NewGuid(),
                    Code = rcptRef,
                    Customer = customer,
                    CustomerId = customer.Id,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    PaymentModeId = hasModeId ? payModeId : (Guid?)null,
                    IsCredit = model.IsCredit,
                    IsWalkIn = model.IsWalkIn,
                    Notes = model.Notes,
                    Personnel = model.Personnel,
                    PersonnelName = model.PersonnelName,
                    PinNo = model.PinNo,
                    PaymentModeNo = model.PaymentModeNo
                };
                if (!model.IsCredit)
                    receipt.AmountReceived = model.AmountReceived;
                if (!model.IsWalkIn && !string.IsNullOrEmpty(model.PinNo))
                {
                    _logger.LogInformation($"{tag} update pin for customer {customer.Id}  {customer.Code} from {customer.PinNo} to {model.PinNo}");
                    customer.PinNo = model.PinNo;
                    customer.DateLastModified = DateTime.Now;
                    customer.LastModifiedBy = model.Personnel;
                }
                var i = 0;
                foreach (var item in lineItems)
                {
                    i++;
                    var product = await _context.Products
                        .Include(p => p.TaxType)
                        .FirstOrDefaultAsync(p => p.Id.Equals(item.ProductId));
                    if (product == null)
                    {
                        result.Message = "Provided product not Found";
                        _logger.LogWarning($"{tag} sale failed {item.ProductId} : {result.Message}");
                        continue;
                    }

                    if (product.AvailableQuantity < item.Quantity)
                    {
                        result.Message = $"{product.Name} available quantity is {product.AvailableQuantity}";
                        _logger.LogWarning($"{tag} sale failed {item.ProductId} : {result.Message}");
                        continue;
                    }

                    var poLineQtyLogs = _context.ProductPoQuantityLogs
                        .Where(p => p.ProductId.Equals(product.Id) && p.AvailableQuantity > 0)
                        .OrderBy(p => p.DateCreated).ToList();
                    var lineItem = new ReceiptLineItem
                    {
                        ReceiptId = receipt.Id,
                        Code = $"{receipt.Code}-{i}",
                        ProductId = product.Id,
                        TaxRate = product.TaxRate,
                        SellingPrice = product.SellingPrice,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        Personnel = receipt.Personnel,
                        ClientId = product.ClientId,
                        InstanceId = product.InstanceId,
                        BuyingPrice = poLineQtyLogs.Any() ?
                        Math.Round(poLineQtyLogs.Sum(p => p.BuyingPrice * p.AvailableQuantity)
                        / poLineQtyLogs.Sum(p => p.AvailableQuantity), 2) : product.BuyingPrice
                    };
                    decimal remainingQty = item.Quantity;
                    foreach (var quantityLog in poLineQtyLogs)
                    {
                        if (remainingQty <= 0)
                            break;
                        if (quantityLog.AvailableQuantity <= remainingQty)
                        {
                            remainingQty -= quantityLog.AvailableQuantity;
                            quantityLog.AvailableQuantity = 0;
                        }
                        if (quantityLog.AvailableQuantity >= remainingQty)
                        {
                            quantityLog.AvailableQuantity -= remainingQty;
                            remainingQty = 0;
                        }
                    }
                    receipt.ReceiptLineItems.Add(lineItem);
                    if (!product.IsService)
                        product.AvailableQuantity -= item.Quantity;
                    product.DateLastModified = DateTime.Now;
                    product.LastModifiedBy = model.Personnel;
                    product.BuyingPrice = lineItem.BuyingPrice;
                }

                if (model.IsCredit)
                {
                    var balancesRes = await GlUserBalanceAsync(GlUserType.Customer, customer.Id);
                    if (!balancesRes.Success)
                    {
                        result.Message = $"{customer.Code} available Limit is {balancesRes.Message}";
                        _logger.LogWarning($"{tag} sale failed {model.CustomerId} : {result.Message}");
                        return result;
                    }
                    var totalAmount = receipt.ReceiptLineItems.Sum(r => r.Amount);
                    var availableLimit = customer.CreditLimit - balancesRes.Data.ExpectedAmount
                        + balancesRes.Data.CreditAmount;
                    if (totalAmount > availableLimit)
                    {
                        result.Message = $"{customer.Code} Limit available is {availableLimit}";
                        _logger.LogWarning($"{tag} sale failed {model.CustomerId} : {result.Message}");
                        return result;
                    }
                }
                _context.Receipts.Add(receipt);
                await _context.SaveChangesAsync();
                await AddInvoiceAsync(receipt);
                if (!model.IsCredit)
                {
                    var entry = new GeneralLedgerEntry
                    {
                        ClientId = receipt.ClientId,
                        InstanceId = receipt.InstanceId,
                        Personnel = receipt.Personnel,
                        UserId = receipt.CustomerId,
                        UserType = GlUserType.Customer,
                        Document = Document.Receipt,
                        DocumentNumber = receipt.Code,
                        DocumentId = receipt.Id,
                        Credit = receipt.Amount,
                        Notes = receipt.Notes,
                        Code = $"{receipt.Code}_{customer.Code}"
                    };
                    _context.GeneralLedgerEntries.Add(entry);
                    await _context.SaveChangesAsync();
                }

                result.Success = true;
                result.Data = receipt;
                result.Message = $"Receipt {receipt.Code} Added";
                _logger.LogInformation($"{tag} sold {i} of {lineItems.Count} products: {result.Message}");
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

        public async Task<ReturnData<PurchaseOrder>> PurchaseOrderByIdAsync(Guid id)
        {
            var result = new ReturnData<PurchaseOrder> { Data = new PurchaseOrder() };
            var tag = nameof(PurchaseOrdersAsync);
            _logger.LogInformation($"{tag} get purchase orders by id {id}");
            try
            {
                var purchaseOrder = await _context.PurchaseOrders
                    .Include(r => r.Supplier)
                    .Include(r => r.PoGrnProducts)
                    .ThenInclude(p => p.Product)
                    .ThenInclude(p => p.TaxType)
                    .FirstOrDefaultAsync(p => p.Id.Equals(id));
                result.Success = purchaseOrder != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = purchaseOrder;
                _logger.LogInformation($"{tag} found purchase order {purchaseOrder.Code}");
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

        public async Task<ReturnData<List<PurchaseOrder>>> PurchaseOrdersAsync(Guid clientId, Guid? instanceId, bool onlyActive = false, string dateFrom = "", string dateTo = "",
            string search = "", string personnel = "")
        {
            var result = new ReturnData<List<PurchaseOrder>> { Data = new List<PurchaseOrder>() };
            var tag = nameof(PurchaseOrdersAsync);
            _logger.LogInformation($"{tag} get purchase orders: clientId {clientId}, instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.PurchaseOrders
                    .Include(r => r.Supplier)
                    .Include(r => r.PoGrnProducts)
                    .Where(r => r.ClientId.Equals(clientId));
                if (instanceId != null)
                    dataQuery = dataQuery.Where(r => r.InstanceId.Equals(instanceId.Value));
                var hasFromDate = DateTime.TryParse(dateFrom, out var dtFrom);
                var hasToDate = DateTime.TryParse(dateTo, out var dtTo);
                if (hasFromDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date >= dtFrom.Date);
                if (hasToDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date <= dtTo.Date);
                if (!string.IsNullOrEmpty(personnel))
                    dataQuery = dataQuery.Where(r => r.Personnel.Equals(personnel));
                if (!string.IsNullOrEmpty(search))
                    dataQuery = dataQuery.Where(r => r.Code.ToLower().Contains(search.ToLower()));
                if (onlyActive)
                    dataQuery = dataQuery.Where(d => d.Status.Equals(EntityStatus.Active));
                var data = await dataQuery.OrderByDescending(r => r.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} purchase orders");
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

        public async Task<ReturnData<List<Receipt>>> ReceiptsAsync(Guid clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "",
            string personnel = "", string modeId = "", string type = "")
        {
            var result = new ReturnData<List<Receipt>> { Data = new List<Receipt>() };
            var tag = nameof(ReceiptsAsync);
            _logger.LogInformation($"{tag} get receipts: clientId {clientId}, instanceId {instanceId}, duration {dateFrom}-{dateTo}," +
                $" search {search}, personnel {personnel}, payment mode {modeId}, type {type}");
            try
            {
                var dataQuery = _context.Receipts
                    .Include(r => r.ReceiptLineItems)
                    .ThenInclude(r => r.Product)
                    .ThenInclude(r => r.ProductCategory)
                    .Include(r => r.Customer)
                    .Where(r => r.ClientId.Equals(clientId));
                if (instanceId != null)
                    dataQuery = dataQuery.Where(r => r.InstanceId.Equals(instanceId.Value));
                var hasFromDate = DateTime.TryParse(dateFrom, out var dtFrom);
                var hasToDate = DateTime.TryParse(dateTo, out var dtTo);
                if (hasFromDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date >= dtFrom.Date);
                if (hasToDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date <= dtTo.Date);
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    dataQuery = dataQuery.Where(r => r.Code.ToLower().Contains(search)
                    || r.Customer.Code.ToLower().Contains(search)
                    || r.Customer.FirstName.ToLower().Contains(search)
                    || r.Customer.LastName.ToLower().Contains(search));
                }
                if (!string.IsNullOrEmpty(personnel))
                {
                    personnel = personnel.ToLower();
                    dataQuery = dataQuery.Where(r => r.Personnel.ToLower().Equals(personnel));
                }
                if (!string.IsNullOrEmpty(type))
                {
                    var isCredit = type.ToLower().Equals("credit");
                    dataQuery = dataQuery.Where(r => r.IsCredit.Equals(isCredit));
                }
                var hasMode = Guid.TryParse(modeId, out var mId);
                if (hasMode)
                    dataQuery = dataQuery.Where(r => r.PaymentModeId.HasValue &&
                    r.PaymentModeId.Value.Equals(mId));

                dataQuery.Where(d => d.ReceiptLineItems.Any());
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

        public async Task<ReturnData<Receipt>> ReceiptByIdAsync(string id)
        {
            var result = new ReturnData<Receipt> { Data = new Receipt() };
            var tag = nameof(ReceiptByIdAsync);
            _logger.LogInformation($"{tag} get receipt by id or code : {id}");
            try
            {
                var hasId = Guid.TryParse(id, out var rId);

                var qry = _context.Receipts
                    .Include(r => r.ReceiptLineItems)
                    .ThenInclude(r => r.Product)
                    .ThenInclude(r => r.ProductCategory)
                    .Include(r => r.Customer).AsQueryable();
                var data = hasId ? await qry.FirstOrDefaultAsync(i => i.Id.Equals(rId))
                   : await qry.FirstOrDefaultAsync(i => i.Code.ToLower().Equals(id.ToLower()));
                result.Success = data != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} {id} {result.Message}");
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

        public async Task<string> AddInvoiceAsync(Receipt receipt)
        {
            var tag = nameof(AddInvoiceAsync);
            _logger.LogInformation($"{tag} add customer invoice. receipt {receipt.Code}, customer {receipt.CustomerId}");
            var invRef = DocumentRefNumber(Document.Invoice, receipt.ClientId);
            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                Code = invRef,
                ClientId = receipt.ClientId,
                ReceiptId = receipt.Id,
                InstanceId = receipt.InstanceId,
                Personnel = receipt.Personnel
            };
            if (!receipt.IsCredit)
                invoice.Status = EntityStatus.Inactive;

            _context.Invoices.Add(invoice);
            var entry = new GeneralLedgerEntry
            {
                ClientId = receipt.ClientId,
                InstanceId = receipt.InstanceId,
                Personnel = receipt.Personnel,
                UserId = receipt.CustomerId,
                UserType = GlUserType.Customer,
                Document = Document.Invoice,
                DocumentNumber = invRef,
                DocumentId = invoice.Id,
                Debit = receipt.Amount,
                Notes = receipt.Notes,
                Code = $"{invRef}_{receipt.Code}"
            };
            _context.GeneralLedgerEntries.Add(entry);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"{tag} invoice {invRef} add for client {receipt.ClientId}");
            return invRef;
        }

        public async Task<string> ReceiptExcessAmount(ReceiptUserViewModel model)
        {
            try
            {
                var id = Guid.NewGuid();
                var code = DocumentRefNumber(Document.Receipt, model.ClientId);
                var defProdId = DefaultClientProductId(model.ClientId);
                var receipt = new Receipt
                {
                    Id = id,
                    Code = code,
                    CustomerId = Guid.Parse(model.UserId),
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    PaymentModeId = Guid.Parse(model.PaymentModeId),
                    PaymentModeNo = model.PaymentModeNo,
                    Notes = model.Notes,
                    Personnel = model.Personnel,
                    PersonnelName = model.PersonnelName,
                    AmountReceived = model.Amount,
                    DateLastModified = DateTime.Now,
                    ReceiptLineItems = new List<ReceiptLineItem>
                    {
                        new ReceiptLineItem
                        {
                            ClientId=model.ClientId,
                            InstanceId=model.InstanceId,
                            Personnel=model.Personnel,
                            Quantity=1,
                            UnitPrice=model.Amount,
                            ReceiptId=id,
                            ProductId=defProdId
                        }
                    }
                };
                _context.Receipts.Add(receipt);
                var entry = new GeneralLedgerEntry
                {
                    ClientId = receipt.ClientId,
                    InstanceId = receipt.InstanceId,
                    Personnel = receipt.Personnel,
                    UserId = receipt.CustomerId,
                    UserType = GlUserType.Customer,
                    Document = Document.Receipt,
                    DocumentNumber = receipt.Code,
                    DocumentId = receipt.Id,
                    Credit = receipt.AmountReceived,
                    Notes = receipt.Notes,
                    Code = $"{receipt.Code}_{receipt.PaymentModeNo}",
                    IsRepayment = true
                };
                _context.GeneralLedgerEntries.Add(entry);
                await _context.SaveChangesAsync();
                return code;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return String.Empty;
            }
        }

        public string DocumentRefNumber(Document document, Guid clientId)
        {
            try
            {
                var nextRef = "";
                var exists = true;
                var i = 0;
                var prefix = "";
                switch (document)
                {
                    case Document.Receipt:
                        prefix = "RCPT";
                        while (exists)
                        {
                            var nextCount = _context.Receipts.Where(r => r.ClientId.Equals(clientId)).Count() + 1;
                            nextRef = prefix + (nextCount + i).ToString("D4");
                            exists = _context.Receipts.Any(a => a.Code.Equals(nextRef) && a.ClientId.Equals(clientId));
                            i++;
                        }
                        break;
                    case Document.Invoice:
                        prefix = "INV";
                        while (exists)
                        {
                            var nextCount = _context.Invoices.Where(r => r.ClientId.Equals(clientId)).Count() + 1;
                            nextRef = prefix + (nextCount + i).ToString("D4");
                            exists = _context.Invoices.Any(a => a.Code.Equals(nextRef) && a.ClientId.Equals(clientId));
                            i++;
                        }
                        break;
                    case Document.Po:
                        prefix = "PO";
                        while (exists)
                        {
                            var nextCount = _context.PurchaseOrders.Where(p => p.ClientId.Equals(clientId)).Count() + 1;
                            nextRef = prefix + (nextCount + i).ToString("D4");
                            exists = _context.PurchaseOrders.Any(a => a.Code.Equals(nextRef) && a.ClientId.Equals(clientId));
                            i++;
                        }
                        break;
                    case Document.Grn:
                        prefix = "SPINV/GRN";
                        while (exists)
                        {
                            var nextCount = _context.GoodReceivedNotes.Where(p => p.ClientId.Equals(clientId)).Count() + 1;
                            nextRef = prefix + (nextCount + i).ToString("D4");
                            exists = _context.GoodReceivedNotes.Any(a => a.Code.Equals(nextRef) && a.ClientId.Equals(clientId));
                            i++;
                        }
                        break;
                    case Document.Customer:
                        prefix = "CUST";
                        while (exists)
                        {
                            var nextCount = _context.Customers.Where(p => p.ClientId.Equals(clientId)).Count() + 1;
                            nextRef = prefix + (nextCount + i).ToString("D4");
                            exists = _context.Customers.Any(a => a.Code.Equals(nextRef) && a.ClientId.Equals(clientId));
                            i++;
                        }
                        break;
                    case Document.Leave:
                        prefix = "LV";
                        while (exists)
                        {
                            var nextCount = _context.EmployeeLeaveApplications.Where(p => p.ClientId.Equals(clientId)).Count() + 1;
                            nextRef = prefix + (nextCount + i).ToString("D4");
                            exists = _context.EmployeeLeaveApplications.Any(a => a.Code.Equals(nextRef) && a.ClientId.Equals(clientId));
                        }
                        break;
                    case Document.Order:
                        prefix = "ORD";
                        while (exists)
                        {
                            var nextCount = _context.Orders.Where(p => p.ClientId.Equals(clientId)).Count() + 1;
                            nextRef = prefix + (nextCount + i).ToString("D4");
                            exists = _context.Orders.Any(a => a.Code.Equals(nextRef) && a.ClientId.Equals(clientId));
                            i++;
                        }
                        break;
                    default:
                        break;
                }
                return nextRef;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                var rand = new Random().Next(99999, 999999999);
                return "R" + rand.ToString("D5");
            }
        }

        public async Task<ReturnData<List<Product>>> LowStockProductsAsync(Guid? instanceId, int limit = 10)
        {
            var result = new ReturnData<List<Product>> { Data = new List<Product>() };
            var tag = nameof(LowStockProductsAsync);
            _logger.LogInformation($"{tag} low stock products. instace id {instanceId}");
            try
            {
                var dataQry = _context.Products
                    .Include(c => c.ProductCategory)
                    .Include(c => c.UnitOfMeasure)
                    .Where(p => p.AvailableQuantity <= p.ReorderLevel)
                    .Where(p => !p.Code.Equals(Constants.DefaultProductCode) && !p.IsService);
                if (instanceId != null)
                    dataQry = dataQry.Where(p => p.InstanceId.Equals(instanceId));
                var data = await dataQry.OrderBy(p => p.AvailableQuantity)
                    .Take(limit)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} products");
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

        public async Task<ReturnData<List<TopSellingProductViewModel>>> TopSellingProductsByVolumeAsync(Guid? instanceId, string dtFrom = "", string dtTo = "", string personnel = "", int limit = 10)
        {
            var result = new ReturnData<List<TopSellingProductViewModel>> { Data = new List<TopSellingProductViewModel>() };
            var tag = nameof(TopSellingProductsByVolumeAsync);
            _logger.LogInformation($"{tag} top {limit} selling products by volume. instace id {instanceId}");
            try
            {
                var dateFrom = DateTime.Parse(dtFrom);
                var dateTo = DateTime.Parse(dtTo);
                var dataQry = _context.ReceiptLineItems
                    .Where(r => r.DateCreated.Date >= dateFrom.Date && r.DateCreated.Date <= dateTo.Date)
                    .GroupBy(l => new { l.ProductId, l.Personnel })
                    .Select(tp => new TopSellingProductViewModel
                    {
                        ProductId = tp.Key.ProductId,
                        Personnel = tp.Key.Personnel,
                        Volume = tp.Count()
                    })
                    .Join(_context.Products.Include(p => p.ProductCategory)
                    .Where(p => p.Status.Equals(EntityStatus.Active)),
                    tp => tp.ProductId, p => p.Id, (tp, p) => new TopSellingProductViewModel
                    {
                        Product = p,
                        ProductId = tp.ProductId,
                        Volume = tp.Volume,
                        InstanceId = p.InstanceId,
                        ClientId = p.ClientId,
                        Personnel = tp.Personnel,
                    })
                    .AsQueryable();
                if (instanceId != null)
                    dataQry = dataQry.Where(p => p.InstanceId.Equals(instanceId));
                if (!string.IsNullOrEmpty(personnel))
                    dataQry = dataQry.Where(p => p.Personnel.Equals(personnel));
                var data = await dataQry.OrderByDescending(p => p.Volume)
                    .Take(limit).ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} products");
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

        public async Task<ReturnData<ProductPriceViewModel>> EditPriceAsync(ProductPriceViewModel model)
        {
            var result = new ReturnData<ProductPriceViewModel> { Data = new ProductPriceViewModel() };
            var priceLogs = new List<ProductPriceLog>();
            var tag = nameof(EditPriceAsync);
            _logger.LogInformation($"{tag} edit product price");
            try
            {
                var productPriceLog = new ProductPriceLog();
                if (!model.ProductPriceMiniViewModels.Any())
                {
                    result.Message = "Not Found";
                    _logger.LogWarning($"{tag} update failed {model.Id} : {result.Message}");
                    return result;
                }

                model.ProductPriceMiniViewModels.ForEach(p =>
               {
                   var product = _context.Products
                   .Where(c => c.ClientId.Equals(model.ClientId) && c.InstanceId.Equals(model.InstanceId))
                   .FirstOrDefault(c => c.Id.Equals(p.Id));
                   if (product != null)
                   {
                       var hasToDate = DateTime.TryParse(p.PriceEndDate, out var endDate);
                       product.PriceStartDate = DateTime.Parse(p.PriceStartDate);
                       product.PriceEndDate = hasToDate ? endDate : (DateTime?)null;
                       productPriceLog.ProductId = product.Id;
                       productPriceLog.PriceStartDate = product.PriceStartDate;
                       productPriceLog.PriceEndDate = product.PriceEndDate;
                       productPriceLog.PriceFrom = product.SellingPrice;
                       product.SellingPrice = p.SellingPrice;
                       productPriceLog.PriceTo = p.SellingPrice;

                       priceLogs.Add(productPriceLog);
                   }
               });

                await _context.ProductPriceLogs.AddRangeAsync(priceLogs);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Updated";

                _logger.LogInformation($"{tag} updated {model.Id} : {result.Message}");
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

        public async Task<ReturnData<string>> PrintReceiptByIdAsync(Guid id, string personnel)
        {
            var tag = nameof(PrintReceiptByIdAsync);
            _logger.LogInformation($"{tag} print original receipt {id} by {personnel}");
            var result = new ReturnData<string> { Data = "" };
            try
            {
                var receipt = await _context.Receipts
                    .FirstOrDefaultAsync(r => r.Id.Equals(id));
                if (receipt == null)
                {
                    result.Message = "Not Found";
                    return result;
                }
                receipt.IsPrinted = true;
                receipt.PrintCount += 1;
                receipt.LastModifiedBy = personnel;
                receipt.DateLastModified = DateTime.Now;
                await _context.SaveChangesAsync();
                result.Message = "Updated";
                result.Success = true;
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

        public async Task<ReturnData<PurchaseOrder>> EditPurchaseOrderAsync(PurchaseOrderViewModel model)
        {
            var result = new ReturnData<PurchaseOrder> { Data = new PurchaseOrder() };
            var tag = nameof(EditPurchaseOrderAsync);
            _logger.LogInformation($"{tag} edit purchase order");
            try
            {
                if (model.IsEditMode)
                {
                    var dbPurchaseOrder = await _context.PurchaseOrders
                            .Include(r => r.Supplier)
                            .Include(r => r.PoGrnProducts)
                            .ThenInclude(r => r.Product)
                            .FirstOrDefaultAsync(p => p.Id.Equals(model.Id));
                    if (dbPurchaseOrder == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogWarning($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    dbPurchaseOrder.Code = model.Code;
                    dbPurchaseOrder.Name = model.Name;
                    dbPurchaseOrder.SupplierId = Guid.Parse(model.SupplierId);
                    dbPurchaseOrder.LastModifiedBy = model.Personnel;
                    dbPurchaseOrder.DateLastModified = DateTime.Now;
                    dbPurchaseOrder.Status = model.Status;
                    if (!dbPurchaseOrder.PoGrnProducts.Any())
                    {
                        result.Message = "Not Found";
                        _logger.LogWarning($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }

                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbPurchaseOrder;
                    _logger.LogInformation($"{tag} updated {dbPurchaseOrder.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var poRef = DocumentRefNumber(Document.Po, model.ClientId);
                var purchaseOrder = new PurchaseOrder
                {
                    Id = Guid.NewGuid(),
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Code = poRef,
                    Name = model.Name,
                    Notes = model.Notes,
                    SupplierId = Guid.Parse(model.SupplierId),
                    Personnel = model.Personnel
                };
                _context.PurchaseOrders.Add(purchaseOrder);
                foreach (var item in model.PurchaseOrderItems)
                {
                    var lineProduct = new PoGrnProduct
                    {
                        PurchaseOrderId = purchaseOrder.Id,
                        ProductId = item.ProductId,
                        PoNotes = item.Notes,
                        PoQuantity = item.Quantity,
                        PoUnitPrice = item.UnitPrice,
                        Personnel = model.Personnel,
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId
                    };
                    _context.PoGrnProducts.Add(lineProduct);
                }

                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Added";
                result.Data = purchaseOrder;
                _logger.LogInformation($"{tag} added {purchaseOrder.Name}  {purchaseOrder.Id} : {result.Message}");
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

        public async Task<ReturnData<List<GoodReceivedNote>>> GoodsReceivedAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "", string personnel = "")
        {
            var result = new ReturnData<List<GoodReceivedNote>> { Data = new List<GoodReceivedNote>() };
            var tag = nameof(GoodsReceivedAsync);
            _logger.LogInformation($"{tag} get goods received note: clientId {clientId}, instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.GoodReceivedNotes
                    .Include(r => r.Supplier)
                    .Include(r => r.PoGrnProducts)
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
                if (!string.IsNullOrEmpty(personnel))
                    dataQuery = dataQuery.Where(r => r.Personnel.Equals(personnel));
                if (!string.IsNullOrEmpty(search))
                    dataQuery = dataQuery.Where(r => r.Code.ToLower().Contains(search.ToLower()));
                var data = await dataQuery.OrderByDescending(r => r.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} goods received note");
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

        public async Task<ReturnData<GoodReceivedNote>> EditGrnAsync(GoodsReceivedNoteViewModel model)
        {
            var result = new ReturnData<GoodReceivedNote> { Data = new GoodReceivedNote() };
            var tag = nameof(EditGrnAsync);
            _logger.LogInformation($"{tag} edit good received note for poid {model.PoId}");
            try
            {
                var purchaseOrder = await _context.PurchaseOrders
                             .Include(p => p.PoGrnProducts)
                             .FirstOrDefaultAsync(p => p.Id.Equals(Guid.Parse(model.PoId)));
                if (purchaseOrder == null)
                {
                    result.Message = "Purchase order Not Found";
                    _logger.LogWarning($"{tag} add GRN failed poId {model.PoId} : {result.Message}");
                    return result;
                }

                var gnrRef = DocumentRefNumber(Document.Grn, model.ClientId);
                var grn = new GoodReceivedNote
                {
                    Id = Guid.NewGuid(),
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Code = gnrRef,
                    Name = model.Name,
                    Notes = model.Notes,
                    SupplierId = Guid.Parse(model.SupplierId),
                    Personnel = model.Personnel,
                    PoCode = purchaseOrder.Code,
                    PoId = purchaseOrder.Id,
                };
                _context.GoodReceivedNotes.Add(grn);

                foreach (var item in model.GrnItems)
                {
                    var dbProduct = _context.Products
                        .FirstOrDefault(d => d.Id.Equals(item.ProductId));
                    if (dbProduct == null)
                        continue;
                    var currentQty = dbProduct.AvailableQuantity;
                    dbProduct.AvailableQuantity += item.Quantity;
                    dbProduct.BuyingPrice = item.UnitPrice;
                    dbProduct.LastModifiedBy = model.Personnel;
                    dbProduct.DateLastModified = DateTime.Now;
                    var log = new ProductStockAdjustmentLog
                    {
                        Code = $"{purchaseOrder.Code}_{dbProduct.Code}",
                        ProductId = item.ProductId,
                        QuantityFrom = currentQty,
                        QuantityTo = dbProduct.AvailableQuantity,
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId,
                        Personnel = model.Personnel,
                        Notes = model.Notes
                    };
                    _context.ProductStockAdjustmentLogs.Add(log);

                    var poQtyLog = new ProductPoQuantityLog
                    {
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId,
                        Personnel = model.Personnel,
                        ProductId = dbProduct.Id,
                        PurchaseOrderId = purchaseOrder.Id,
                        DeliveredQuantity = item.Quantity,
                        AvailableQuantity = dbProduct.AvailableQuantity,
                        BuyingPrice = item.UnitPrice,
                        Code = $"{purchaseOrder.Code}_{dbProduct.Code}"
                    };
                    _context.ProductPoQuantityLogs.Add(poQtyLog);

                    grn.PoGrnProducts.Add(new PoGrnProduct
                    {
                        ClientId = model.ClientId,
                        InstanceId = model.InstanceId,
                        Personnel = model.Personnel,
                        ProductId = item.ProductId,
                        GrnQuantity = item.Quantity,
                        GrnUnitPrice = item.UnitPrice,
                        GrnNotes = item.Notes,
                        GoodReceivedNoteId = grn.Id,
                        Code = $"{purchaseOrder.Code}_{dbProduct.Code}"
                    });

                    var dbGrnProduct = purchaseOrder.PoGrnProducts
                        .FirstOrDefault(p => p.ProductId.Equals(item.ProductId));
                    if (dbGrnProduct != null)
                    {
                        dbGrnProduct.GrnQuantity += item.Quantity;
                        dbGrnProduct.GrnUnitPrice += item.UnitPrice;
                        dbGrnProduct.LastModifiedBy = model.Personnel;
                        dbGrnProduct.DateLastModified = DateTime.Now;
                        dbGrnProduct.GrnNotes = item.Notes;
                    }
                    _context.SaveChanges();
                }

                if (purchaseOrder.PoGrnProducts.Sum(p => p.PoQtyBalance) <= model.GrnItems.Sum(s => s.Quantity))
                {
                    purchaseOrder.Status = EntityStatus.Closed;
                    purchaseOrder.LastModifiedBy = model.Personnel;
                    purchaseOrder.DateLastModified = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                result.Success = true;
                result.Message = $"Added {grn.Code}";
                result.Data = grn;
                _logger.LogInformation($"{tag} added {grn.Name}  {grn.Id} : {result.Message}");
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

        public async Task<ReturnData<GoodReceivedNote>> GrnByIdAsync(Guid id)
        {
            var result = new ReturnData<GoodReceivedNote> { Data = new GoodReceivedNote() };
            var tag = nameof(GrnByIdAsync);
            _logger.LogInformation($"{tag} get grn by id {id}");
            try
            {
                var grn = await _context.GoodReceivedNotes
                    .Include(r => r.Supplier)
                    .Include(p => p.PoGrnProducts)
                    .ThenInclude(pr => pr.Product)
                    .FirstOrDefaultAsync(p => p.Id.Equals(id));
                result.Success = grn != null;
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = grn;
                _logger.LogInformation($"{tag} found received goods");
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
        public async Task<ReturnData<ProductPriceLog>> ProductPriceAsync(ItemPriceViewModel model)
        {
            var result = new ReturnData<ProductPriceLog> { Data = new ProductPriceLog() };
            var tag = nameof(ProductPriceAsync);
            _logger.LogInformation($"{tag} set price for product {model.ProductId}");
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id.Equals(Guid.Parse(model.ProductId)));
                if (product == null)
                {
                    result.Message = "Not Found";
                    _logger.LogWarning($"{tag} price set failed {model.ProductId} : {result.Message}");
                    return result;
                }
                var hasToDate = DateTime.TryParse(model.PriceEndDateStr, out var endDate);
                var currentPrice = product.SellingPrice;
                product.SellingPrice = model.SellingPrice;
                var hasFromDate = DateTime.TryParse(model.PriceStartDateStr, out var startDate);
                product.PriceStartDate = hasFromDate ? startDate : DateTime.Now;
                product.PriceEndDate = hasToDate ? endDate : (DateTime?)null;
                var log = new ProductPriceLog
                {
                    Code = $"{product.Code}",
                    ProductId = product.Id,
                    PriceFrom = currentPrice,
                    PriceTo = model.SellingPrice,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Notes = model.Notes,
                    PriceStartDate = product.PriceStartDate,
                    PriceEndDate = product.PriceEndDate,
                };
                _context.ProductPriceLogs.Add(log);
                product.LastModifiedBy = model.Personnel;
                product.DateLastModified = DateTime.Now;
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Price Set";
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

        public async Task<ReturnData<ProductDataViewModel>> ProductDetailsAsync(string productCode, Guid instanceId)
        {
            var tag = nameof(ProductDetailsAsync);
            _logger.LogInformation($"{tag} get product data for {productCode} in {instanceId}");
            var result = new ReturnData<ProductDataViewModel> { Data = new ProductDataViewModel() };
            try
            {
                if (string.IsNullOrEmpty(productCode))
                {
                    result.Message = result.Data.Message = "Product code is required";
                    return result;
                }
                productCode = productCode.ToLower();
                var product = await _context.Products
                    .Include(p => p.ProductCategory)
                    .Include(p => p.TaxType)
                    .Include(p => p.UnitOfMeasure)
                    .Where(p => p.InstanceId.Equals(instanceId))
                    .Where(p => p.Code.ToLower().Equals(productCode))
                    .FirstOrDefaultAsync();
                if (product == null)
                {
                    result.Message = result.Data.Message = $" {productCode} Not found";
                    _logger.LogWarning($"{tag} get product details failed {productCode} : {result.Message}");
                    return result;
                }

                result.Success = result.Data.Success = true;
                result.Message = result.Data.Message = "Found";
                result.Data.Product = product;
                result.Data.ProductStockAdjustmentLogs = await _context.ProductStockAdjustmentLogs
                    .Where(a => a.ProductId.Equals(product.Id))
                    .OrderByDescending(a => a.DateCreated)
                    .ToListAsync();
                result.Data.ProductPriceLogs = await _context.ProductPriceLogs
                    .Where(a => a.ProductId.Equals(product.Id))
                    .OrderByDescending(a => a.DateCreated)
                    .ToListAsync();
                result.Data.TotalSales = await _context.ReceiptLineItems
                    .Where(r => r.ProductId.Equals(product.Id))
                    .SumAsync(r => r.Quantity * r.UnitPrice);
                result.Data.TotalQtySold = await _context.ReceiptLineItems
                    .Where(r => r.ProductId.Equals(product.Id))
                    .SumAsync(r => r.Quantity);
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

        public async Task<ReturnData<List<GeneralLedgerEntry>>> GeneralLedgersAsync(Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "")
        {
            var result = new ReturnData<List<GeneralLedgerEntry>> { Data = new List<GeneralLedgerEntry>() };
            var tag = nameof(GeneralLedgersAsync);
            _logger.LogInformation($"{tag} get general ledger: instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.GeneralLedgerEntries.AsQueryable();
                if (instanceId != null)
                    dataQuery = dataQuery.Where(r => r.InstanceId.Equals(instanceId.Value));
                var prevDataQuery = dataQuery;
                var hasFromDate = DateTime.TryParse(dateFrom, out var dtFrom);
                var hasToDate = DateTime.TryParse(dateTo, out var dtTo);
                if (hasFromDate)
                {
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date >= dtFrom.Date);
                    prevDataQuery = prevDataQuery.Where(r => r.DateCreated.Date < dtFrom.Date);
                }
                if (hasToDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date <= dtTo.Date);
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    dataQuery = dataQuery.Where(r => r.Code.ToLower().Contains(search));
                    prevDataQuery = prevDataQuery.Where(r => r.Code.ToLower().Contains(search));
                }

                var prevLedger = new GeneralLedgerEntry
                {
                    DateCreated = dtFrom.AddDays(-1),
                    Code = $"BALANCE_{dateFrom}",
                    Personnel = "SYS",
                    UserType = GlUserType.Customer,
                    Credit = prevDataQuery.Sum(s => s.Credit),
                    Debit = prevDataQuery.Sum(s => s.Debit),
                    Id = Guid.Empty
                };

                var data = await dataQuery.ToListAsync();
                data.Add(prevLedger);
                data.OrderBy(d => d.DateCreated);
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
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

        public async Task<ReturnData<string>> ReceiptUserAsync(ReceiptUserViewModel model)
        {
            var result = new ReturnData<string> { Data = "" };
            var tag = nameof(ReceiptUserAsync);
            _logger.LogInformation($"{tag} receipt {model.UserType} {model.UserId}: clientId {model.ClientId}, instanceId {model.InstanceId}, amount {model.Amount}");
            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.Receipt)
                    .ThenInclude(i => i.ReceiptLineItems)
                    .Where(u => u.Receipt.CustomerId.Equals(Guid.Parse(model.UserId)))
                    .Where(i => i.Status.Equals(EntityStatus.Active))
                    .OrderBy(i => i.DateCreated)
                    .ToListAsync();
                if (!invoices.Any())
                {
                    var res = await ReceiptExcessAmount(model);
                    result.Data = res;
                    result.Success = !string.IsNullOrEmpty(res);
                    result.Message = result.Success ? "Receipted" : "Not receipted";
                    return result;
                }
                decimal remainingAmount = model.Amount + model.AvailableCredit;
                foreach (var invoice in invoices)
                {
                    if (remainingAmount > 0)
                    {
                        var toSpend = remainingAmount < invoice.Balance ? remainingAmount : invoice.Balance;
                        if (toSpend <= 0) continue;

                        invoice.LastModifiedBy = model.Personnel;
                        invoice.DateLastModified = DateTime.Now;
                        invoice.Receipt.AmountReceived += toSpend;
                        invoice.Receipt.LastModifiedBy = model.Personnel;
                        invoice.Receipt.PaymentModeId = Guid.Parse(model.PaymentModeId);
                        invoice.Receipt.DateLastModified = DateTime.Now;
                        if (invoice.Balance <= 0)
                            invoice.Status = EntityStatus.Closed;
                        remainingAmount -= toSpend;
                        var entry = new GeneralLedgerEntry
                        {
                            ClientId = model.ClientId,
                            InstanceId = model.InstanceId,
                            UserId = Guid.Parse(model.UserId),
                            UserType = model.UserType,
                            Credit = toSpend,
                            Document = Document.Receipt,
                            DocumentNumber = invoice.Receipt.Code,
                            DocumentId = invoice.Receipt.Id,
                            Code = $"{invoice.Receipt.Code}_{invoice.Code}",
                            Personnel = model.Personnel,
                            IsRepayment = true
                        };
                        _context.GeneralLedgerEntries.Add(entry);
                    }
                    _context.SaveChanges();
                }

                if (remainingAmount > 0)
                {
                    model.Amount = remainingAmount;
                    await ReceiptExcessAmount(model);
                }
                result.Success = true;
                result.Message = "Receipted";
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

        public Guid DefaultClientProductId(Guid clientId)
        {
            return _context.Products
                        .Where(p => p.ClientId.Equals(clientId))
                        .OrderByDescending(p => p.DateCreated)
                        .Select(p => p.Id)
                        .FirstOrDefault();
        }

        public async Task<ReturnData<ReceiptUserViewModel>> GlUserBalanceAsync(GlUserType userType, Guid userId)
        {
            var tag = nameof(GlUserBalanceAsync);
            _logger.LogInformation($"{tag} get {userType} {userId}  balance");
            var result = new ReturnData<ReceiptUserViewModel>
            {
                Data = new ReceiptUserViewModel
                {
                    UserId = userId.ToString(),
                    UserType = userType
                }
            };
            try
            {
                var debit = await _context.GeneralLedgerEntries
                    .Where(u => u.UserId.Equals(userId))
                    .SumAsync(l => l.Debit);
                var credit = await _context.GeneralLedgerEntries
                    .Where(u => u.UserId.Equals(userId))
                    .SumAsync(l => l.Credit);
                result.Success = true;
                result.Message = "Found";
                result.Data.CreditAmount = credit;
                result.Data.DebitAmount = debit;
                _logger.LogInformation($"{tag} {result.Message} : credit= {credit} debit= {debit}");
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

        public async Task<ReturnData<List<ProductPoQuantityLog>>> ProductPoQuantityLogAsync(Guid clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "")
        {
            var result = new ReturnData<List<ProductPoQuantityLog>> { Data = new List<ProductPoQuantityLog>() };
            var tag = nameof(ProductPoQuantityLogAsync);
            _logger.LogInformation($"{tag} get p.o quantity log: clientId {clientId}, instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.ProductPoQuantityLogs
                    .Include(r => r.Product)
                    .Include(r => r.PurchaseOrder)
                    .Where(r => r.ClientId.Equals(clientId));
                if (instanceId != null)
                    dataQuery = dataQuery.Where(r => r.InstanceId.Equals(instanceId.Value));
                var hasFromDate = DateTime.TryParse(dateFrom, out var dtFrom);
                var hasToDate = DateTime.TryParse(dateTo, out var dtTo);
                if (hasFromDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date >= dtFrom.Date);
                if (hasToDate)
                    dataQuery = dataQuery.Where(r => r.DateCreated.Date <= dtTo.Date);
                if (!string.IsNullOrEmpty(search))
                    dataQuery = dataQuery.Where(r => r.Product.Code.ToLower().Contains(search.ToLower()));
                var data = await dataQuery.OrderByDescending(r => r.DateCreated)
                    .ToListAsync();
                result.Success = data.Any();
                result.Message = result.Success ? "Found" : "Not Found";
                if (result.Success)
                    result.Data = data;
                _logger.LogInformation($"{tag} found {data.Count} logs");
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

        public async Task<ReturnData<string>> CancelReceiptAsync(CancelReceiptViewModel model)
        {
            var result = new ReturnData<string>();
            var tag = nameof(CancelReceiptAsync);
            _logger.LogInformation($"{tag} client {model.ClientId} cancel receipt {model.Code} by {model.Personnel}");

            try
            {
                if (string.IsNullOrEmpty(model.Code))
                {
                    result.Message = "Receipt number required";
                    return result;
                }
                var receiptNo = model.Code.Trim().ToLower();
                var receipt = await _context.Receipts
                    .Include(r => r.ReceiptLineItems)
                    .FirstOrDefaultAsync(r => r.Code.ToLower().Equals(receiptNo));
                if (receipt == null)
                {
                    result.Message = "Provided receipt not Found";
                    return result;
                }

                foreach (var item in receipt.ReceiptLineItems)
                {
                    var product = _context.Products.FirstOrDefault(p => p.Id.Equals(item.ProductId));
                    if (product != null)
                    {
                        product.AvailableQuantity += item.Quantity;
                        product.LastModifiedBy = model.Personnel;
                        product.DateLastModified = DateTime.Now;
                    }
                }
                var ledgers = _context.GeneralLedgerEntries
                    .Where(g => g.DocumentId.Equals(receipt.Id)).ToList();
                _context.GeneralLedgerEntries.RemoveRange(ledgers);

                receipt.Status = EntityStatus.Inactive;
                receipt.LastModifiedBy = model.Personnel;
                receipt.Notes += $"{model.Notes}";
                receipt.DateLastModified = DateTime.Now;
                _context.ReceiptLineItems.RemoveRange(receipt.ReceiptLineItems);
                var glEntries = _context.GeneralLedgerEntries
                    .Join(_context.Invoices.Where(i => i.ReceiptId.Equals(receipt.Id)), l => l.DocumentId,
                    i => i.Id, (l, i) => l).ToList();
                _context.GeneralLedgerEntries.RemoveRange(glEntries);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Receipt cancelled";
                result.Data = receiptNo;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occurred";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }

        public async Task<ReturnData<string>> PaySupplierGrnAsync(ExpenseViewModel model)
        {
            var result = new ReturnData<string>();
            var tag = nameof(PaySupplierGrnAsync);
            _logger.LogInformation($"{tag} client {model.ClientId} pay supplier {model.SupplierId} for {model.Code}  amount {model.Amount} by {model.Personnel}");
            try
            {

                var grn = await _context.GoodReceivedNotes
                    .Include(g => g.PoGrnProducts)
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.GrnId));
                if (grn == null)
                {
                    result.Message = "Provided document not Found";
                    return result;
                }

                if (model.Amount > grn.Balance)
                {
                    result.Message = $"Available balance is {grn.Balance}";
                    return result;
                }

                grn.AmountReceived += model.Amount;
                grn.LastModifiedBy = model.Personnel;
                grn.DateLastModified = DateTime.Now;
                if (grn.AmountReceived >= grn.Amount)
                    grn.Status = EntityStatus.Closed;

                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Payment success";
                result.Data = grn.Code;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result.ErrorMessage = ex.Message;
                result.Message = "Error occurred";
                _logger.LogError($"{tag} {result.Message} : {ex}");
                return result;
            }
        }
    }
}
