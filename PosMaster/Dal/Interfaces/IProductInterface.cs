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
        Task<ReturnData<List<Receipt>>> ReceiptsAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "");
        Task<ReturnData<ProductStockAdjustmentLog>> AdjustProductStockAsync(ProductStockAdjustmentViewModel model);
        Task<ReturnData<PurchaseOrder>> AddPurchaseOrderAsync(PoViewModel model);
        Task<ReturnData<List<PurchaseOrder>>> PurchaseOrdersAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "", string personnel = "");
        Task<ReturnData<PurchaseOrder>> PurchaseOrderByIdAsync(Guid id);
        string DocumentRefNumber(Document document, Guid clientId);
        Task<ReturnData<List<Product>>> LowStockProductsAsync(Guid? clientId, Guid? instanceId, int limit = 10);
        Task<ReturnData<List<TopSellingProductViewModel>>> TopSellingProductsByVolumeAsync(Guid? clientId, Guid? instanceId, int limit = 10);
        Task<ReturnData<ProductPriceViewModel>> EditPriceAsync(ProductPriceViewModel model);
        Task<ReturnData<Receipt>> ReceiptByIdAsync(Guid id);
        Task<ReturnData<PurchaseOrder>> EditPurchaseOrderAsync(PurchaseOrderViewModel model);
        Task<ReturnData<string>> PrintReceiptByIdAsync(Guid id, string personnel);
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
                        InstanceId = model.InstanceId
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
                var count = _context.ProductStockAdjustmentLogs.Count(p => p.ProductId.Equals(product.Id));
                var log = new ProductStockAdjustmentLog
                {
                    Code = $"{product.Code}-{count + 1}",
                    ProductId = product.Id,
                    QuantityFrom = product.AvailableQuantity,
                    QuantityTo = model.QuantityTo,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Notes = model.Notes
                };
                product.AvailableQuantity = model.QuantityTo;
                product.LastModifiedBy = model.Personnel;
                product.DateLastModified = DateTime.Now;
                _context.ProductStockAdjustmentLogs.Add(log);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Adjusted";
                result.Data = log;
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
            _logger.LogInformation($"{tag} get all instance {instanceId} products");
            try
            {
                var dataQry = _context.Products
                    .Include(c => c.ProductCategory)
                    .Where(c => c.ClientId.Equals(clientId))
                    .AsQueryable();
                if (instanceId != null)
                    dataQry = dataQry.Where(d => d.InstanceId.Equals(instanceId.Value));
                if (isPos)
                {
                    dataQry = dataQry.Where(d => d.AvailableQuantity > 0 && d.SellingPrice > 0)
                    .Where(d => d.PriceStartDate.Date <= DateTime.Now.Date && (d.PriceEndDate == null || d.PriceEndDate.Value.Date >= DateTime.Now.Date));
                }
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    dataQry = dataQry.Where(d => d.Code.ToLower().Contains(search)
                    || d.Name.ToLower().Contains(search))
                    .Take(10);
                }
                var data = await dataQry.OrderByDescending(c => c.DateCreated)
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

        public async Task<ReturnData<Product>> EditAsync(ProductViewModel model)
        {
            var result = new ReturnData<Product> { Data = new Product() };
            var tag = nameof(EditAsync);
            _logger.LogInformation($"{tag} edit product");
            try
            {
                var hasTaxTypeId = Guid.TryParse(model.TaxTypeId, out var taxTypeId);
                if (model.IsEditMode)
                {
                    var dbProduct = await _context.Products
                        .FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
                    if (dbProduct == null)
                    {
                        result.Message = "Not Found";
                        _logger.LogWarning($"{tag} update failed {model.Id} : {result.Message}");
                        return result;
                    }
                    if (model.AvailableQuantity != dbProduct.AvailableQuantity)
                    {
                        var adjustLog = new ProductStockAdjustmentLog
                        {
                            ClientId = dbProduct.ClientId,
                            InstanceId = dbProduct.InstanceId,
                            ProductId = dbProduct.Id,
                            QuantityFrom = dbProduct.AvailableQuantity,
                            QuantityTo = model.AvailableQuantity,
                            Personnel = model.Personnel,
                            Notes = "Product Edit"
                        };
                        _context.ProductStockAdjustmentLogs.Add(adjustLog);
                    }
                    dbProduct.Code = model.Code;
                    dbProduct.ProductCategoryId = Guid.Parse(model.ProductCategoryId);
                    dbProduct.Name = model.Name;
                    dbProduct.ReorderLevel = model.ReorderLevel;
                    dbProduct.BuyingPrice = model.BuyingPrice;
                    dbProduct.SellingPrice = model.SellingPrice;
                    dbProduct.AllowDiscount = model.AllowDiscount;
                    dbProduct.AvailableQuantity = model.AvailableQuantity;
                    dbProduct.UnitOfMeasure = model.UnitOfMeasure;
                    dbProduct.LastModifiedBy = model.Personnel;
                    dbProduct.DateLastModified = DateTime.Now;
                    dbProduct.Notes = model.Notes;
                    dbProduct.Status = model.Status;
                    dbProduct.TaxTypeId = hasTaxTypeId ? taxTypeId : (Guid?)null;
                    if (model.IsNewImage)
                        dbProduct.ImagePath = model.ImagePath;

                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Updated";
                    result.Data = dbProduct;
                    _logger.LogInformation($"{tag} updated {dbProduct.Name} {model.Id} : {result.Message}");
                    return result;
                }

                var product = new Product
                {
                    Code = model.Code,
                    ProductCategoryId = Guid.Parse(model.ProductCategoryId),
                    Name = model.Name,
                    ReorderLevel = model.ReorderLevel,
                    BuyingPrice = model.BuyingPrice,
                    SellingPrice = model.SellingPrice,
                    AllowDiscount = model.AllowDiscount,
                    AvailableQuantity = model.AvailableQuantity,
                    UnitOfMeasure = model.UnitOfMeasure,
                    Notes = model.Notes,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    Personnel = model.Personnel,
                    Status = model.Status,
                    ImagePath = model.ImagePath,
                    TaxTypeId = hasTaxTypeId ? taxTypeId : (Guid?)null
                };
                _context.Products.Add(product);
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
                var receipt = new Receipt
                {
                    Id = Guid.NewGuid(),
                    Code = rcptRef,
                    Customer = customer,
                    CustomerId = customer.Id,
                    ClientId = model.ClientId,
                    InstanceId = model.InstanceId,
                    PaymentMode = model.PaymentMode,
                    PinNo = string.IsNullOrEmpty(model.PinNo) ? customer.PinNo : model.PinNo,
                    IsCredit = model.IsCredit,
                    IsWalkIn = model.IsWalkIn,
                    Notes = model.Notes,
                    Personnel = model.Personnel
                };
                receipt.IsPaid = !model.IsCredit;
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
                        BuyingPrice = product.BuyingPrice
                    };
                    receipt.ReceiptLineItems.Add(lineItem);
                    product.AvailableQuantity -= item.Quantity;
                    product.DateLastModified = DateTime.Now;
                    product.LastModifiedBy = model.Personnel;
                }

                if (model.IsCredit && receipt.ReceiptLineItems.Sum(r => r.Amount) > customer.CreditLimit)
                {
                    result.Message = $"{customer.Code} Limit is {customer.CreditLimit}";
                    _logger.LogWarning($"{tag} sale failed {model.CustomerId} : {result.Message}");
                    return result;
                }

                _context.Receipts.Add(receipt);
                await _context.SaveChangesAsync();

                if (model.IsCredit)
                    await AddInvoiceAsync(receipt);

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

        public async Task<ReturnData<List<PurchaseOrder>>> PurchaseOrdersAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "", string personnel = "")
        {
            var result = new ReturnData<List<PurchaseOrder>> { Data = new List<PurchaseOrder>() };
            var tag = nameof(PurchaseOrdersAsync);
            _logger.LogInformation($"{tag} get purchase orders: clientId {clientId}, instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.PurchaseOrders
                    .Include(r => r.Supplier)
                    .Include(r => r.PoGrnProducts)
                    //.ThenInclude(p => p.Product)
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

        public async Task<ReturnData<List<Receipt>>> ReceiptsAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "")
        {
            var result = new ReturnData<List<Receipt>> { Data = new List<Receipt>() };
            var tag = nameof(ReceiptsAsync);
            _logger.LogInformation($"{tag} get receipts: clientId {clientId}, instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
            try
            {
                var dataQuery = _context.Receipts
                    .Include(r => r.ReceiptLineItems)
                    .ThenInclude(r => r.Product)
                    .ThenInclude(r => r.ProductCategory)
                    .Include(r => r.Customer)
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

        public async Task<ReturnData<Receipt>> ReceiptByIdAsync(Guid id)
        {
            var result = new ReturnData<Receipt> { Data = new Receipt() };
            var tag = nameof(ReceiptByIdAsync);
            _logger.LogInformation($"{tag} get receipt by id : {id}");
            try
            {
                var data = await _context.Receipts
                    .Include(r => r.ReceiptLineItems)
                    .ThenInclude(r => r.Product)
                    .ThenInclude(r => r.ProductCategory)
                    .Include(r => r.Customer)
                    .FirstOrDefaultAsync(i => i.Id.Equals(id));
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

        private async Task<string> AddInvoiceAsync(Receipt receipt)
        {
            var tag = nameof(AddInvoiceAsync);
            _logger.LogInformation($"{tag} add customer invoice. receipt {receipt.Code}, customer {receipt.CustomerId}");
            var invRef = DocumentRefNumber(Document.Invoice, receipt.ClientId);
            var invoice = new Invoice
            {
                Code = invRef,
                ClientId = receipt.ClientId,
                ReceiptId = receipt.Id,
                InstanceId = receipt.InstanceId,
                Personnel = receipt.Personnel
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"{tag} invoice {invRef} add for client {receipt.ClientId}");
            return invRef;
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
                        prefix = "GRN";
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

        public async Task<ReturnData<List<Product>>> LowStockProductsAsync(Guid? clientId, Guid? instanceId, int limit = 10)
        {
            var result = new ReturnData<List<Product>> { Data = new List<Product>() };
            var tag = nameof(LowStockProductsAsync);
            _logger.LogInformation($"{tag} low stock products. client id {clientId} and instace id {instanceId}");
            try
            {
                var dataQry = _context.Products
                    .Include(c => c.ProductCategory)
                    .Where(p => p.AvailableQuantity <= p.ReorderLevel)
                    .AsQueryable();
                if (clientId != null)
                    dataQry = dataQry.Where(d => d.ClientId.Equals(clientId));
                if (instanceId != null)
                    dataQry = dataQry.Where(p => p.InstanceId.Equals(instanceId));
                var data = await dataQry.OrderBy(p => p.AvailableQuantity)
                    .Take(limit)
                    .ToListAsync();
                if (!data.Any())
                {
                    var dataQry_ = _context.Products
                    .Include(c => c.ProductCategory)
                    .AsQueryable();
                    if (clientId != null)
                        dataQry = dataQry.Where(d => d.ClientId.Equals(clientId));
                    if (instanceId != null)
                        dataQry_ = dataQry_.Where(p => p.InstanceId.Equals(instanceId));
                    data = await dataQry_.OrderBy(p => p.AvailableQuantity)
                      .Take(limit)
                      .ToListAsync();
                }
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

        public async Task<ReturnData<List<TopSellingProductViewModel>>> TopSellingProductsByVolumeAsync(Guid? clientId, Guid? instanceId, int limit = 10)
        {
            var result = new ReturnData<List<TopSellingProductViewModel>> { Data = new List<TopSellingProductViewModel>() };
            var tag = nameof(TopSellingProductsByVolumeAsync);
            _logger.LogInformation($"{tag} top {limit} selling products by volume. client id {clientId} and instace id {instanceId}");
            try
            {
                var dataQry = _context.ReceiptLineItems
                    .GroupBy(l => l.ProductId)
                    .Select(tp => new TopSellingProductViewModel
                    {
                        ProductId = tp.Key,
                        Volume = tp.Count()
                    })
                    .Join(_context.Products.Include(p => p.ProductCategory),
                    tp => tp.ProductId, p => p.Id, (tp, p) => new TopSellingProductViewModel
                    {
                        Product = p,
                        ProductId = tp.ProductId,
                        Volume = tp.Volume,
                        InstanceId = p.InstanceId,
                        ClientId = p.ClientId
                    })
                    .AsQueryable();
                if (clientId != null)
                    dataQry = dataQry.Where(p => p.ClientId.Equals(clientId));
                if (instanceId != null)
                    dataQry = dataQry.Where(p => p.InstanceId.Equals(instanceId));
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

                var dataQry = await _context.Products
                    .Where(c => c.ClientId.Equals(model.ClientId) && c.InstanceId.Equals(model.InstanceId)).ToListAsync();
                model.ProductPriceMiniViewModels.ForEach(p =>
                {
                    var product = dataQry.FirstOrDefault(c => c.Id.Equals(p.Id));
                    if (product != null)
                    {
                        var hasToDate = DateTime.TryParse(p.PriceEndDate, out var endDate);
                        product.SellingPrice = p.SellingPrice;
                        product.PriceStartDate = DateTime.Parse(p.PriceStartDate);
                        product.PriceEndDate = hasToDate ? endDate : (DateTime?)null;

                        productPriceLog.ProductId = product.Id;
                        productPriceLog.PriceStartDate = product.PriceStartDate;
                        productPriceLog.PriceEndDate = product.PriceEndDate;
                        productPriceLog.PriceFrom = product.SellingPrice;
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
                    var dbProduct = await _context.Products.FirstOrDefaultAsync(d => d.Id.Equals(item.ProductId));
                    dbProduct.BuyingPrice = item.UnitPrice;

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
    }
}
