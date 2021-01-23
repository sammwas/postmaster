using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace PosMaster.Services
{
	public interface ICookiesService
	{
		UserCookieData Store(UserCookieData data);
		UserCookieData Read();
		void Remove();
	}

	public class CookiesService : ICookiesService
	{
		private readonly string _key = "USER_DATA";
		private readonly IHttpContextAccessor _httpContextAccessor;
		public CookiesService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}
		public UserCookieData Read()
		{
			var data = _httpContextAccessor.HttpContext.Request.Cookies[_key];
			return JsonConvert.DeserializeObject<UserCookieData>(data);
		}

		public void Remove()
		{
			_httpContextAccessor.HttpContext.Response.Cookies.Delete(_key);
		}

		public UserCookieData Store(UserCookieData data)
		{
			var options = new CookieOptions
			{
				//Expires = DateTime.Now.AddSeconds(10)
			};
			_httpContextAccessor.HttpContext.Response.Cookies.Append(_key, JsonConvert.SerializeObject(data));
			return data;
		}
	}
}
