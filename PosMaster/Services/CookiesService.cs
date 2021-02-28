using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

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
		private readonly string _key = ".PosMaster.User";
		private readonly IHttpContextAccessor _httpContextAccessor;
		public CookiesService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}
		public UserCookieData Read()
		{
			var data = _httpContextAccessor.HttpContext.Request.Cookies[_key];
			if (string.IsNullOrEmpty(data))
				return new UserCookieData();
			var encoded = Helpers.Base64Decode(data);
			return JsonConvert.DeserializeObject<UserCookieData>(EncypterService.Decrypt(encoded));
		}

		public void Remove()
		{
			_httpContextAccessor.HttpContext.Response.Cookies.Delete(_key);
			//foreach (var cookie in _httpContextAccessor.HttpContext.Request.Cookies)
			//{
			//	_httpContextAccessor.HttpContext.Response.Cookies.Delete(cookie.Key);
			//}
		}

		public UserCookieData Store(UserCookieData data)
		{
			var options = new CookieOptions
			{
				//Expires = DateTime.Now.AddSeconds(10)
				SameSite = SameSiteMode.Strict,
				HttpOnly = true,
				//Secure = true
			};
			var rawData = EncypterService.Encrypt(JsonConvert.SerializeObject(data));
			_httpContextAccessor.HttpContext.Response.Cookies.Append(_key, Helpers.Base64Encode(rawData), options);
			return data;
		}
	}
}
