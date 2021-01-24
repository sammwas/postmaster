﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
		Task<ReturnData<List<Product>>> AllAsync();
		Task<ReturnData<List<Product>>> ByClientIdAsync(Guid clientId);
		Task<ReturnData<List<Product>>> ByInstanceIdAsync(Guid instanceId);
		Task<ReturnData<Product>> ByIdAsync(Guid id);
		Task<ReturnData<Receipt>> ProductSaleAsync(ProductSaleViewModel model);
		Task<ReturnData<List<Receipt>>> ReceiptsAsync(Guid? clientId, Guid? instanceId, string dateFrom = "", string dateTo = "", string search = "");
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

		public async Task<ReturnData<List<Product>>> AllAsync()
		{
			var result = new ReturnData<List<Product>> { Data = new List<Product>() };
			var tag = nameof(AllAsync);
			_logger.LogInformation($"{tag} get all products");
			try
			{
				var data = await _context.Products
					.Include(c => c.ProductCategory)
					.OrderByDescending(c => c.DateCreated)
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

		public async Task<ReturnData<List<Product>>> ByClientIdAsync(Guid clientId)
		{
			var result = new ReturnData<List<Product>> { Data = new List<Product>() };
			var tag = nameof(ByClientIdAsync);
			_logger.LogInformation($"{tag} get all client {clientId} products");
			try
			{
				var data = await _context.Products
					.Include(c => c.ProductCategory)
					.Where(c => c.ClientId.Equals(clientId))
					.OrderByDescending(c => c.DateCreated)
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

		public async Task<ReturnData<List<Product>>> ByInstanceIdAsync(Guid instanceId)
		{
			var result = new ReturnData<List<Product>> { Data = new List<Product>() };
			var tag = nameof(ByInstanceIdAsync);
			_logger.LogInformation($"{tag} get all instance {instanceId} products");
			try
			{
				var data = await _context.Products
					.Include(c => c.ProductCategory)
					.Where(c => c.InstanceId.Equals(instanceId))
					.OrderByDescending(c => c.DateCreated)
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
					ImagePath = model.ImagePath
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

		public async Task<ReturnData<Receipt>> ProductSaleAsync(ProductSaleViewModel model)
		{
			var result = new ReturnData<Receipt> { Data = new Receipt() };
			var tag = nameof(ProductSaleAsync);
			_logger.LogInformation($"{tag} sell {model.Quantity} of product {model.ProductId} at {model.UnitPrice}");
			try
			{
				var customer = model.IsWalkIn ?
					await _context.Customers.FirstOrDefaultAsync(c => c.Code.Equals(Constants.WalkInCustomerCode) && c.ClientId.Equals(model.ClientId))
					: await _context.Customers.FirstOrDefaultAsync(c => c.Id.Equals(Guid.Parse(model.CustomerId)));
				if (customer == null)
				{
					result.Message = "Provided customer not Found";
					_logger.LogWarning($"{tag} sale failed {model.ProductId} : {result.Message}");
					return result;
				}
				var product = await _context.Products.FirstOrDefaultAsync(p => p.Id.Equals(Guid.Parse(model.ProductId)));
				if (product == null)
				{
					result.Message = "Provided product not Found";
					_logger.LogWarning($"{tag} sale failed {model.ProductId} : {result.Message}");
					return result;
				}

				if (product.AvailableQuantity < model.Quantity)
				{
					result.Message = $"{product.Name} available quantity is {product.AvailableQuantity}";
					_logger.LogWarning($"{tag} sale failed {model.ProductId} : {result.Message}");
					return result;
				}

				var receipt = new Receipt
				{
					UnitPrice = model.UnitPrice,
					Discount = model.Discount,
					Customer = customer,
					CustomerId = customer.Id,
					ClientId = model.ClientId,
					InstanceId = model.InstanceId,
					Product = product,
					ProductId = product.Id,
					Quantity = model.Quantity,
					PaymentMode = model.PaymentMode,
					ExternalRef = model.ExternalRef,
					IsCredit = model.IsCredit,
					IsWalkIn = model.IsWalkIn,
					Notes = model.Notes,
					Personnel = model.Personnel
				};
				product.AvailableQuantity -= model.Quantity;
				product.DateLastModified = DateTime.Now;
				product.LastModifiedBy = model.Personnel;
				_context.Receipts.Add(receipt);
				await _context.SaveChangesAsync();

				result.Success = true;
				result.Data = receipt;
				result.Message = $"Receipt {receipt.Code} Added";
				_logger.LogInformation($"{tag} product  {model.ProductId} sold : {result.Message}");
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
				var dataQuery = _context.Receipts.Include(r => r.Product)
					.ThenInclude(p => p.ProductCategory)
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
	}
}
