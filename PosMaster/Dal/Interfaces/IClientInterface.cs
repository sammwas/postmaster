using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
	public interface IClientInterface
	{
		Task<ReturnData<Client>> EditAsync(ClientViewModel model);
		Task<ReturnData<List<Client>>> AllAsync();
		Task<ReturnData<Client>> ByIdAsync(Guid id);
	}


	public class ClientImplementation : IClientInterface
	{
		private readonly DatabaseContext _context;
		private readonly ILogger<ClientImplementation> _logger;
		public ClientImplementation(DatabaseContext context, ILogger<ClientImplementation> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<ReturnData<List<Client>>> AllAsync()
		{
			var result = new ReturnData<List<Client>> { Data = new List<Client>() };
			var tag = nameof(AllAsync);
			_logger.LogInformation($"{tag} get all clients");
			try
			{
				var data = await _context.Clients
					.OrderByDescending(c => c.DateCreated)
					.ToListAsync();
				result.Success = data.Any();
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = data;
				_logger.LogInformation($"{tag} found {data.Count} clients");
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

		public async Task<ReturnData<Client>> ByIdAsync(Guid id)
		{
			var result = new ReturnData<Client> { Data = new Client() };
			var tag = nameof(ByIdAsync);
			_logger.LogInformation($"{tag} get client by id - {id}");
			try
			{
				var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id.Equals(id));
				result.Success = client != null;
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = client;
				_logger.LogInformation($"{tag} client {id} {result.Message}");
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

		public async Task<ReturnData<Client>> EditAsync(ClientViewModel model)
		{
			var result = new ReturnData<Client> { Data = new Client() };
			var tag = nameof(EditAsync);
			_logger.LogInformation($"{tag} edit client");
			try
			{
				if (model.IsEditMode)
				{
					var dbClient = await _context.Clients.FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
					if (dbClient == null)
					{
						result.Message = "Not Found";
						_logger.LogInformation($"{tag} update failed {model.Id} : {result.Message}");
						return result;
					}
					dbClient.Code = model.Code;
					dbClient.Name = model.Name;
					dbClient.Slogan = model.Slogan;
					dbClient.CurrencyFull = model.CurrencyFull;
					dbClient.CurrencyShort = model.CurrencyShort;
					dbClient.CountryFull = model.CountryFull;
					dbClient.CountryShort = model.CountryShort;
					dbClient.Vision = model.Vision;
					dbClient.Mission = model.Mission;
					dbClient.EnforcePassword = model.EnforcePassword;
					dbClient.PasswordExpiryMonths = model.PasswordExpiryMonths;
					dbClient.PostalAddress = model.PostalAddress;
					dbClient.Town = model.Town;
					dbClient.EmailAddress = model.EmailAddress;
					dbClient.PrimaryColor = model.PrimaryColor;
					dbClient.SecondaryColor = model.SecondaryColor;
					dbClient.PrimaryTelephone = model.PrimaryTelephone;
					dbClient.SecondaryTelephone = model.SecondaryTelephone;
					dbClient.PhoneNumberLength = model.PhoneNumberLength;
					dbClient.TelephoneCode = model.TelephoneCode;
					dbClient.DisplayBuyingPrice = model.DisplayBuyingPrice;
					dbClient.Notes = model.Notes;
					dbClient.Status = model.Status;
					dbClient.DateLastModified = DateTime.UtcNow;
					dbClient.LastModifiedBy = model.Personnel;
					if (model.IsNewImage)
						dbClient.LogoPath = model.LogoPath;
					await _context.SaveChangesAsync();
					result.Data = dbClient;
					result.Success = true;
					result.Message = "Updated";
					_logger.LogInformation($"{tag} updated {dbClient.Name} {model.Id} : {result.Message}");
					return result;
				}
				var client = new Client
				{
					Code = model.Code,
					Name = model.Name,
					Slogan = model.Slogan,
					CurrencyFull = model.CurrencyFull,
					CurrencyShort = model.CurrencyShort,
					Vision = model.Vision,
					Mission = model.Mission,
					EnforcePassword = model.EnforcePassword,
					PasswordExpiryMonths = model.PasswordExpiryMonths,
					PostalAddress = model.PostalAddress,
					Town = model.Town,
					EmailAddress = model.EmailAddress,
					PrimaryColor = model.PrimaryColor,
					SecondaryColor = model.SecondaryColor,
					PrimaryTelephone = model.PrimaryTelephone,
					SecondaryTelephone = model.SecondaryTelephone,
					PhoneNumberLength = model.PhoneNumberLength,
					TelephoneCode = model.TelephoneCode,
					DisplayBuyingPrice = model.DisplayBuyingPrice,
					Personnel = model.Personnel,
					Status = model.Status,
					LogoPath = model.LogoPath,
					Notes = model.Notes
				};
				client.ClientId = client.InstanceId = client.Id;
				var instance = new ClientInstance
				{
					ClientId = client.Id,
					Code = "MAIN",
					Name = "MAIN",
					Personnel = model.Personnel,
					Status = EntityStatus.Active
				};
				instance.InstanceId = instance.Id;
				_context.Clients.Add(client);
				_context.ClientInstances.Add(instance);
				await _context.SaveChangesAsync();
				result.Success = true;
				result.Message = "Added";
				_logger.LogInformation($"{tag} added {client.Name}  {client.Id} : {result.Message}");
				result.Data = client;
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


