using MailKit.Security;
using PosMaster.Dal;
using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PosMaster.Services
{
	public static class Helpers
	{
		public static DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		public static DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
		public static DateTime FirstDayOfWeek()
		{
			var dayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
			int delta = dayOfWeek - DateTime.Now.DayOfWeek;
			return DateTime.Now.AddDays(delta);
		}
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

		public static List<FormSelectViewModel> SocketOptions()
		{
			var statusesList = Enum.GetValues(typeof(SecureSocketOptions))
				.Cast<SecureSocketOptions>()
				.ToList();
			return statusesList.Select(s => new FormSelectViewModel
			{
				Id = s.ToString(),
				Text = s.ToString()
			}).ToList();
		}

		public static List<string> UserRoles(string currentRole)
		{
			var roles = new List<string> { Role.Clerk, currentRole };
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

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>
(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				if (seenKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
		}

		public static IEnumerable<Tuple<string, int, int>> MonthsBetween(
	   DateTime startDate,
	   DateTime endDate)
		{
			DateTime iterator;
			DateTime limit;
			if (endDate > startDate)
			{
				iterator = new DateTime(startDate.Year, startDate.Month, 1);
				limit = endDate;
			}
			else
			{
				iterator = new DateTime(endDate.Year, endDate.Month, 1);
				limit = startDate;
			}

			var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
			while (iterator <= limit)
			{
				yield return Tuple.Create(
					dateTimeFormat.GetMonthName(iterator.Month),
					iterator.Year, DateTime.DaysInMonth(iterator.Year, iterator.Month));
				iterator = iterator.AddMonths(1);
			}
		}
	}
}
