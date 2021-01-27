using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class SystemSettingMiniViewModel
	{
		public SystemSettingMiniViewModel()
		{

		}

		public SystemSettingMiniViewModel(SystemSetting setting)
		{
			Name = setting.Name;
			Tagline = setting.Tagline;
			Description = setting.Description;
			Version = setting.Version;
			PhoneNumber = setting.PhoneNumber;
			EmailAddress = setting.EmailAddress;
			PostalAddress = setting.PostalAddress;
			Town = setting.Town;
			LogoPath = setting.LogoPath;
		}
		public string Name { get; set; }
		public string Tagline { get; set; }
		public string Description { get; set; }
		public string Version { get; set; }
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }
		[Display(Name = "Email Address")]
		public string EmailAddress { get; set; }
		[Display(Name = "Postal Address")]
		public string PostalAddress { get; set; }
		public string Town { get; set; }
		public string LogoPath { get; set; }
	}
}
