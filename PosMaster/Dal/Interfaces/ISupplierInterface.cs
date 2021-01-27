using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
	public interface ISupplierInterface
	{
		Task<ReturnData<Supplier>> EditAsync(SupplierViewModel model);
		Task<ReturnData<List<Supplier>>> AllAsync();
		Task<ReturnData<List<Supplier>>> ByClientIdAsync(Guid clientId);
		Task<ReturnData<Supplier>> ByIdAsync(Guid id);
	}

	public class SupplierImplementation : ISupplierInterface
	{
		private readonly DatabaseContext _context;
		private readonly ILogger<SupplierImplementation> _logger;
		public SupplierImplementation(DatabaseContext context, ILogger<SupplierImplementation> logger)
		{
			_context = context;
			_logger = logger;
		}
		public async Task<ReturnData<List<Supplier>>> AllAsync()
		{
			var result = new ReturnData<List<Supplier>> { Data = new List<Supplier>() };
			var tag = nameof(AllAsync);
			_logger.LogInformation($"{tag} get all suppliers");
			try
			{
				var data = await _context.Suppliers
					.OrderByDescending(c => c.DateCreated)
					.ToListAsync();
				result.Success = data.Any();
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = data;
				_logger.LogInformation($"{tag} found {data.Count} suppliers");
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

		public async Task<ReturnData<List<Supplier>>> ByClientIdAsync(Guid clientId)
		{
			var result = new ReturnData<List<Supplier>> { Data = new List<Supplier>() };
			var tag = nameof(ByClientIdAsync);
			_logger.LogInformation($"{tag} get suppliers for client {clientId}");
			try
			{
				var data = await _context.Suppliers
					.Where(c => c.ClientId.Equals(clientId))
					.OrderByDescending(c => c.DateCreated)
					.ToListAsync();
				result.Success = data.Any();
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = data;
				_logger.LogInformation($"{tag} found {data.Count} suppliers");
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

		public async Task<ReturnData<Supplier>> ByIdAsync(Guid id)
		{
			var result = new ReturnData<Supplier> { Data = new Supplier() };
			var tag = nameof(ByIdAsync);
			_logger.LogInformation($"{tag} get supplier by id - {id}");
			try
			{
				var data = await _context.Suppliers.FirstOrDefaultAsync(c => c.Id.Equals(id));
				result.Success = data != null;
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = data;
				_logger.LogInformation($"{tag} supplier {id} {result.Message}");
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

		public async Task<ReturnData<Supplier>> EditAsync(SupplierViewModel model)
		{
			var result = new ReturnData<Supplier> { Data = new Supplier() };
			var tag = nameof(EditAsync);
			_logger.LogInformation($"{tag} edit supplier");
			try
			{
				if (model.IsEditMode)
				{
					var dbSupplier = await _context.Suppliers
						.FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
					if (dbSupplier == null)
					{
						result.Message = "Not Found";
						_logger.LogWarning($"{tag} update failed {model.Id} : {result.Message}");
						return result;
					}
					dbSupplier.Code = model.Code;
					dbSupplier.Name = model.Name;
					dbSupplier.PrimaryTelephone = model.PrimaryTelephone;
					dbSupplier.SecondaryTelephone = model.SecondaryTelephone;
					dbSupplier.EmailAddress = model.EmailAddress;
					dbSupplier.Website = model.Website;
					dbSupplier.Location = model.Location;
					dbSupplier.Town = model.Town;
					dbSupplier.PostalAddress = model.PostalAddress;
					dbSupplier.LastModifiedBy = model.Personnel;
					dbSupplier.DateLastModified = DateTime.Now;
					dbSupplier.Notes = model.Notes;
					dbSupplier.Status = model.Status;
					await _context.SaveChangesAsync();
					result.Success = true;
					result.Message = "Updated";
					result.Data = dbSupplier;
					_logger.LogInformation($"{tag} updated {dbSupplier.Name} {model.Id} : {result.Message}");
					return result;
				}
				var supplier = new Supplier
				{
					Code = model.Code,
					Notes = model.Notes,
					ClientId = model.ClientId,
					InstanceId = model.InstanceId,
					Personnel = model.Personnel,
					Status = model.Status,
					PrimaryTelephone = model.PrimaryTelephone,
					SecondaryTelephone = model.SecondaryTelephone,
					EmailAddress = model.EmailAddress,
					PostalAddress = model.PostalAddress,
					Location = model.Location,
					Town = model.Town,
					Website = model.Website
				};
				_context.Suppliers.Add(supplier);
				await _context.SaveChangesAsync();
				result.Success = true;
				result.Message = "Added";
				result.Data = supplier;
				_logger.LogInformation($"{tag} added {supplier.Name} {supplier.Code}  {supplier.Id} : {result.Message}");
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
