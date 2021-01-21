using Microsoft.EntityFrameworkCore;
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
		Task<ReturnData<Client>> AllAsync(int pageNumber, int perPage, int currentPage, string search);
		Task<ReturnData<Client>> ByIdAsync(Guid id);
	}


	public class ClientImplementation : IClientInterface
	{
		private readonly DatabaseContext _context;
		public ClientImplementation(DatabaseContext context)
		{
			_context = context;
		}

		public Task<ReturnData<Client>> AllAsync(int pageNumber, int perPage, int currentPage, string search)
		{
			throw new NotImplementedException();
		}

		public async Task<ReturnData<Client>> ByIdAsync(Guid id)
		{
			var result = new ReturnData<Client> { Data = new Client() };
			try
			{
				var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id.Equals(id));
				result.Success = client != null;
				result.Message = result.Success ? "Found" : "Not Found";
				if (result.Success)
					result.Data = client;
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				result.ErrorMessage = ex.Message;
				result.Message = "Error occured";
				return result;
			}
		}

		public async Task<ReturnData<Client>> EditAsync(ClientViewModel model)
		{
			var result = new ReturnData<Client> { Data = new Client() };
			try
			{
				if (model.IsEditMode)
				{
					var dbClient = await _context.Clients.FirstOrDefaultAsync(c => c.Id.Equals(model.Id));
					if (dbClient == null)
					{
						result.Message = "Not Found";
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
					dbClient.DateLastModified = DateTime.UtcNow;
					dbClient.LastModifiedBy = model.Personnel;
					if (model.IsNewImage)
						dbClient.LogoPath = model.NewImagePath;
					await _context.SaveChangesAsync();
					result.Data = dbClient;
					result.Success = true;
					result.Message = "Updated";
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
				_context.Clients.Add(client);
				await _context.SaveChangesAsync();
				result.Success = true;
				result.Message = "Added";
				result.Data = client;
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				result.ErrorMessage = ex.Message;
				result.Message = "Error occured";
				return result;
			}
		}
	}
}


