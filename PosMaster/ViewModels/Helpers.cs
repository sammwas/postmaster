using PosMaster.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.ViewModels
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
	}
}
