using Microsoft.EntityFrameworkCore;
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
		Task<ReturnData<Product>> ByIdAsync(Guid id);
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

		public async Task<ReturnData<Product>> ByIdAsync(Guid id)
		{
			var result = new ReturnData<Product> { Data = new Product() };
			var tag = nameof(ByIdAsync);
			_logger.LogInformation($"{tag} get product by id - {id}");
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
						_logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
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
						dbProduct.ImagePath = model.NewImagePath;
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
	}
}
