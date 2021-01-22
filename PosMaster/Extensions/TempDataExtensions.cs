using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PosMaster.Dal;

namespace PosMaster.Extensions
{
	public static class TempDataExtensions
	{
		public static void SetData(this ITempDataDictionary @this, AlertLevel type, string title, string message)
		{
			@this["message"] = message;
			@this["title"] = title;
			@this["type"] = type.ToString();
		}
		 
	}
}
