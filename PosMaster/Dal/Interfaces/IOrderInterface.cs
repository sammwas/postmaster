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
	public interface IOrderInterface
	{
		Task<ReturnData<List<Order>>> OrdersAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "");
		Task<ReturnData<Order>> PlaceOrderAsync(OrderViewModel model);
		Task<ReturnData<Order>> OrderByIdAsync(Guid id);
		Task<ReturnData<Order>> EditAsync(OrderViewModel model);
		Task<ReturnData<Receipt>> FulfillOrder(Guid id);
	}
	public class OrdersImplementation : IOrderInterface
	{
		private readonly IProductInterface _productInterface;
		private readonly DatabaseContext _context;
		private readonly ILogger<OrdersImplementation> _logger;
		public OrdersImplementation(DatabaseContext context, ILogger<OrdersImplementation> logger, IProductInterface productInterface)
		{
			_context = context;
			_logger = logger;
			_productInterface = productInterface;
		}
		public async Task<ReturnData<List<Order>>> OrdersAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "")
		{
			var result = new ReturnData<List<Order>> { Data = new List<Order>() };
			var tag = nameof(OrdersAsync);
			_logger.LogInformation($"{tag} get orders: clientId {clientId}, instanceId {instanceId}, duration {dateFrom}-{dateTo}, search {search}");
			try
			{
				var dataQuery = _context.Orders
					.Include(r => r.OrderLineItems)
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

		public async Task<ReturnData<Order>> PlaceOrderAsync(OrderViewModel model)
		{
			var result = new ReturnData<Order> { Data = new Order() };
			var tag = nameof(PlaceOrderAsync);
			_logger.LogInformation($"{tag} create purchase order for instance {model.InstanceId}");
			try
			{
				var lineItems = string.IsNullOrEmpty(model.LineItemListStr) ?
				new List<OrderLineItemMiniViewModel>()
				: JsonConvert.DeserializeObject<List<OrderLineItemMiniViewModel>>(model.LineItemListStr);
				if (!lineItems.Any())
				{
					result.Message = "No line items found";
					_logger.LogWarning($"{tag} order failed  {model.InstanceId} : {result.Message}");
					return result;
				}

				var orderRef = DocumentRefNumber(Document.Order, model.ClientId);

				var order = new Order
				{
					Id = Guid.NewGuid(),
					ClientId = model.ClientId,
					InstanceId = model.InstanceId,
					Code = orderRef,
					Name = model.Name,
					Notes = model.Notes,
					Personnel = model.Personnel,
					CustomerId = Guid.Parse(model.CustomerId),
				};

				var i = 0;
				foreach (var item in lineItems)
				{
					i++;
					var product = await _context.Products.FirstOrDefaultAsync(p => p.Id.Equals(item.ProductId));
					if (product == null)
					{
						result.Message = "Provided product not Found";
						_logger.LogWarning($"{tag} sale failed {item.ProductId} : {result.Message}");
						continue;
					}

					if (product.AvailableQuantity < item.Quantity)
					{
						result.Message = $"{product.Name} available quantity is {product.AvailableQuantity}";
						_logger.LogWarning($"{tag} order failed {item.ProductId} : {result.Message}");
						continue;
					}
					var lineItem = new OrderLineItem
					{
						OrderId = order.Id,
						Code = $"{order.Code}-{i}",
						ProductId = product.Id,
						TaxRate = product.TaxRate,
						SellingPrice = product.SellingPrice,
						UnitPrice = item.UnitPrice,
						Quantity = item.Quantity,
						Discount = item.Discount,
						Personnel = order.Personnel,
						ClientId = order.ClientId,
						InstanceId = order.InstanceId,
						BuyingPrice = product.BuyingPrice,
						LastModifiedBy = model.Personnel
					};
					order.OrderLineItems.Add(lineItem);
				}

				order.DateLastModified = DateTime.Now;
				order.LastModifiedBy = model.Personnel;
				_context.Orders.Add(order);
				await _context.SaveChangesAsync();
				result.Success = true;
				result.Data = order;
				result.Message = $"Order {orderRef} Added";
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

		public async Task<ReturnData<Order>> OrderByIdAsync(Guid id)
		{
			var result = new ReturnData<Order> { Data = new Order() };
			var tag = nameof(OrderByIdAsync);
			_logger.LogInformation($"{tag} get orders by id {id}");
			try
			{
				var order = await _context.Orders
					.Include(r => r.Customer)
					.Include(r => r.OrderLineItems)
					.ThenInclude(p => p.Product)
					.FirstOrDefaultAsync(p => p.Id.Equals(id));
				result.Success = order != null;
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = order;
				_logger.LogInformation($"{tag} found order {order.Code}");
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
		public async Task<ReturnData<Order>> EditAsync(OrderViewModel model)
		{
			var result = new ReturnData<Order> { Data = new Order() };
			var tag = nameof(EditAsync);
			_logger.LogInformation($"{tag} edit order");
			try
			{
				var dbOrder = await _context.Orders.Include(o => o.OrderLineItems)
					.FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
				if (dbOrder == null)
				{
					result.Message = "Not Found";
					_logger.LogWarning($"{tag} update failed {model.Id} : {result.Message}");
					return result;
				}
				var lineItems = string.IsNullOrEmpty(model.LineItemListStr) ?
					new List<OrderLineItemMiniViewModel>()
					: JsonConvert.DeserializeObject<List<OrderLineItemMiniViewModel>>(model.LineItemListStr);
				if (!lineItems.Any())
				{
					result.Message = "No line items found";
					_logger.LogWarning($"{tag} order failed  {model.InstanceId} : {result.Message}");
					return result;
				}

				foreach (var item in lineItems)
				{
					var rmItem = dbOrder.OrderLineItems.FirstOrDefault(o => o.ProductId == item.ProductId);
					dbOrder.OrderLineItems.Remove(rmItem);

					var product = await _context.Products.FirstOrDefaultAsync(p => p.Id.Equals(item.ProductId));
					if (product == null)
					{
						result.Message = "Provided product not Found";
						_logger.LogWarning($"{tag} sale failed {item.ProductId} : {result.Message}");
						continue;
					}

					if (product.AvailableQuantity < item.Quantity)
					{
						result.Message = $"{product.Name} available quantity is {product.AvailableQuantity}";
						_logger.LogWarning($"{tag} order failed {item.ProductId} : {result.Message}");
						continue;
					}
					var lineItem = new OrderLineItem
					{
						OrderId = dbOrder.Id,
						Code = model.Code,
						ProductId = product.Id,
						TaxRate = product.TaxRate,
						SellingPrice = product.SellingPrice,
						UnitPrice = item.UnitPrice,
						Quantity = item.Quantity,
						Discount = item.Discount,
						Personnel = model.Personnel,
						ClientId = model.ClientId,
						InstanceId = dbOrder.InstanceId,
						BuyingPrice = product.BuyingPrice,
						LastModifiedBy = model.Personnel
					};
					dbOrder.OrderLineItems.Add(lineItem);
				}

				dbOrder.DateLastModified = DateTime.Now;
				dbOrder.LastModifiedBy = model.Personnel;
				dbOrder.Status = model.Status;

				await _context.SaveChangesAsync();
				result.Success = true;
				result.Data = dbOrder;
				result.Message = $"Order {model.Code} Updated";
				_logger.LogInformation($"{tag} updated {lineItems.Count} products: {result.Message}");
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

        public async Task<ReturnData<Receipt>> FulfillOrder(Guid id)
        {
			var result = new ReturnData<Receipt> { Data = new Receipt() };
			var tag = nameof(FulfillOrder);
			_logger.LogInformation($"{tag} fulfil order");
            try
            {
				var order = await _context.Orders
							.Include(o => o.OrderLineItems)
							.ThenInclude(o => o.Product)
							.FirstOrDefaultAsync(o => o.Id == id);
				if (order == null) 
				{
					result.Message = "Order was not found";
					_logger.LogWarning($"{tag} failed: {result.Message}");
					return result;
				}
				var model = new OrderViewModel(order);
				if (string.IsNullOrEmpty(model.LineItemListStr))
				{
					result.Message = "No line items found";
					_logger.LogWarning($"{tag} order failed  {model.InstanceId} : {result.Message}");
					return result;
				}
				var saleObj = new ProductSaleViewModel
				{ 
					ClientId = model.ClientId,
					Code = model.Code,
					InstanceId = model.InstanceId,
					CustomerId = model.CustomerId,
					Personnel = model.Personnel,
					LineItemListStr = model.LineItemListStr,
					PaymentMode = "CASH"
				};
				var receipt = await _productInterface.ProductsSaleAsync(saleObj);
				if (!receipt.Success) 
				{
					result.Success = false;
					result.Message = receipt.Message;
					_logger.LogError($"{tag} {result.Message}");
					return result;
				}

				order.Status = EntityStatus.Closed;
				order.DateLastModified = DateTime.Now; 
				await _context.SaveChangesAsync();
				result = receipt;
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
