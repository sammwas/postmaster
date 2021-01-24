using PosMaster.Dal;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosMaster.Services
{
	public static class Helpers
	{
		public static string FormatAmount(decimal amount, int dec = 0)
		{
			var format = "{0:n" + dec + "}";
			return string.Format($"{format}", amount);
		}

		public static List<FormSelectViewModel> Statuses()
		{
			var statusesList = Enum.GetValues(typeof(EntityStatus))
				.Cast<EntityStatus>()
				.ToList();
			return statusesList.Select(s => new FormSelectViewModel
			{
				Id = s.ToString(),
				Text = s.ToString()
			}).ToList();
		}

		public static List<string> UserRoles(string currentRole)
		{
			var roles = new List<string> { Role.Clerk };
			if (currentRole.Equals(Role.Admin))
				return roles;
			if (currentRole.Equals(Role.Manager))
				roles.Add(Role.Admin);
			if (currentRole.Equals(Role.SuperAdmin))
				roles.AddRange(new List<string> { Role.Manager, Role.Admin });
			return roles;
		}

		public static string Base64Encode(string plainText)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

		public static string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(base64EncodedBytes);
		}
	}
}
