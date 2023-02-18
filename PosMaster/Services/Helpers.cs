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
        public static DateTime firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
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

        public static List<FormSelectViewModel> GlUserTypes()
        {
            var statusesList = Enum.GetValues(typeof(GlUserType))
                .Cast<GlUserType>()
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

        public static bool StringContains(string main, string search)
        {
            if (string.IsNullOrEmpty(main))
                return false;
            return main.ToLower().Contains(search.ToLower());
        }

        public static List<string> userTitles = new List<string> { "Mr.", "Mrs.", "Ms." };

        public static string RelativeTime(DateTime dateTime)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - dateTime.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }
    }
}
