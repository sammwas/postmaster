using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
	public interface IUserInterface
	{
		Task<ReturnData<string>> AddLoginLogAsync(UserLoginLog log);
	}
	public class UserInterface : IUserInterface
	{
		private readonly DatabaseContext _context;
		private readonly ILogger<UserInterface> _logger;
		public UserInterface(DatabaseContext context, ILogger<UserInterface> logger)
		{
			_context = context;
			_logger = logger;
		}
		public async Task<ReturnData<string>> AddLoginLogAsync(UserLoginLog log)
		{
			var result = new ReturnData<string>();
			var tag = nameof(AddLoginLogAsync);
			_logger.LogInformation($"{tag} add login log for {log.UserName}");
			try
			{
				if (log.Success)
				{
					var user = _context.Users
						.FirstOrDefault(u => u.Email.ToLower().Equals(log.UserName.ToLower()));
					if (user != null)
					{
						user.LastLoginTime = DateTime.Now;
						_context.SaveChanges();
					}
				}
				await _context.UserLoginLogs.AddAsync(log);
				await _context.SaveChangesAsync();
				result.Success = true;
				result.Message = "Added";
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
