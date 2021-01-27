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
		Task<ReturnData<SystemSettingMiniViewModel>> Update(SystemSettingMiniViewModel model, string personnel);
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
				var dbSettings = await _context.SystemSettings.Take(1).FirstOrDefaultAsync();
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

		public Task<ReturnData<SystemSettingMiniViewModel>> Update(SystemSettingMiniViewModel model, string personnel)
		{
			throw new NotImplementedException();
		}
	}
}
