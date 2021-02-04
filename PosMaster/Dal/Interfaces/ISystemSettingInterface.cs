using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PosMaster.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
	public interface ISystemSettingInterface
	{

		Task<ReturnData<SystemSettingMiniViewModel>> ReadAsync();
		Task<ReturnData<SystemSettingMiniViewModel>> UpdateAsync(SystemSettingMiniViewModel model, string personnel);
		Task<ReturnData<TermsAndPrivacyViewModel>> TermsAndPrivacyAsync();
		Task<ReturnData<TermsAndPrivacyViewModel>> UpdateTermsAndPrivacyAsync(TermsAndPrivacyViewModel model, string personnel);
	}

	public class SystemSettingImplementation : ISystemSettingInterface
	{
		private readonly DatabaseContext _context;
		private readonly ILogger<SystemSettingImplementation> _logger;
		private readonly IMemoryCache _cache;
		private readonly string _key = "SYS_SETTINGS";
		public SystemSettingImplementation(DatabaseContext context, ILogger<SystemSettingImplementation> logger,
				  IMemoryCache cache)
		{
			_context = context;
			_logger = logger;
			_cache = cache;
		}
		public async Task<ReturnData<SystemSettingMiniViewModel>> ReadAsync()
		{
			var result = new ReturnData<SystemSettingMiniViewModel> { Data = new SystemSettingMiniViewModel() };
			var tag = nameof(ReadAsync);
			_logger.LogInformation($"{tag} get system settings");
			try
			{
				var hasValue = _cache.TryGetValue<string>(_key, out var settings);
				if (hasValue)
				{
					result.Success = true;
					result.Message = "Found-Cached";
					result.Data = JsonConvert.DeserializeObject<SystemSettingMiniViewModel>(settings);
					_logger.LogInformation($"{tag} {result.Message}");
					return result;
				}
				var dbSettings = await _context.SystemSettings.FirstOrDefaultAsync();
				var data = dbSettings == null ? new SystemSettingMiniViewModel { Name = "PosMaster" }
				: new SystemSettingMiniViewModel(dbSettings);
				result.Success = true;
				result.Message = "Found-Db";
				result.Data = data;
				_cache.Set(_key, JsonConvert.SerializeObject(data));
				_logger.LogInformation($"{tag} {result.Message}");
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

		public async Task<ReturnData<TermsAndPrivacyViewModel>> TermsAndPrivacyAsync()
		{
			var result = new ReturnData<TermsAndPrivacyViewModel> { Data = new TermsAndPrivacyViewModel() };
			var tag = nameof(TermsAndPrivacyAsync);
			_logger.LogInformation($"{tag} get system terms and conditions");
			try
			{
				var tp = await _context.SystemSettings
					.Select(s => new TermsAndPrivacyViewModel
					{
						Terms = s.TermsAndConditions,
						Privacy = s.Privacy
					})
					.FirstOrDefaultAsync();
				result.Success = tp != null;
				if (result.Success)
					result.Data = tp;
				result.Message = result.Success ? "Found" : "Not Found";
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

		public async Task<ReturnData<SystemSettingMiniViewModel>> UpdateAsync(SystemSettingMiniViewModel model, string personnel)
		{
			var result = new ReturnData<SystemSettingMiniViewModel> { Data = new SystemSettingMiniViewModel() };
			var tag = nameof(UpdateAsync);
			_logger.LogInformation($"{tag} update system settings");
			try
			{
				var settings = await _context.SystemSettings.Take(1).FirstAsync();
				settings.Name = model.Name;
				settings.Tagline = model.Tagline;
				settings.Description = model.Description;
				settings.Version = model.Version;
				settings.PhoneNumber = model.PhoneNumber;
				settings.EmailAddress = model.EmailAddress;
				settings.PostalAddress = model.PostalAddress;
				settings.Town = model.Town;
				if (model.IsNewImage)
					settings.LogoPath = model.LogoPath;
				settings.DateLastModified = DateTime.Now;
				settings.LastModifiedBy = personnel;
				await _context.SaveChangesAsync();
				result.Success = true;
				result.Message = "Updated";
				result.Data = new SystemSettingMiniViewModel(settings);
				_cache.Set(_key, JsonConvert.SerializeObject(result.Data));
				_logger.LogInformation($"{tag} {result.Message}");
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

		public async Task<ReturnData<TermsAndPrivacyViewModel>> UpdateTermsAndPrivacyAsync(TermsAndPrivacyViewModel model, string personnel)
		{
			var result = new ReturnData<TermsAndPrivacyViewModel> { Data = new TermsAndPrivacyViewModel() };
			var tag = nameof(UpdateTermsAndPrivacyAsync);
			_logger.LogInformation($"{tag} update system terms and conditions by {personnel}");
			try
			{
				var setting = await _context.SystemSettings.FirstOrDefaultAsync();
				if (setting == null)
				{
					result.Message = "Not Found";
					return result;
				}

				setting.TermsAndConditions = model.Terms;
				setting.Privacy = model.Privacy;
				setting.LastModifiedBy = personnel;
				setting.DateLastModified = DateTime.Now;
				await _context.SaveChangesAsync();
				result.Success = true;
				if (result.Success)
					result.Data = model;
				result.Message = "Updated";
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
