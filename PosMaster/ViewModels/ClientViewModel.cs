using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class ClientViewModel : BaseViewModel
	{
		public ClientViewModel()
		{

		}

		public ClientViewModel(Client client)
		{
			Id = client.Id;
			Code = client.Code;
			Name = client.Name;
			Slogan = client.Slogan;
			CurrencyFull = client.CurrencyFull;
			CurrencyShort = client.CurrencyShort;
			CountryFull = client.CountryFull;
			CountryShort = client.CountryShort;
			Vision = client.Vision;
			Mission = client.Mission;
			LogoPath = client.LogoPath;
			EnforcePassword = client.EnforcePassword;
			PasswordExpiryMonths = client.PasswordExpiryMonths;
			PostalAddress = client.PostalAddress;
			Town = client.Town;
			EmailAddress = client.EmailAddress;
			PrimaryTelephone = client.PrimaryTelephone;
			SecondaryTelephone = client.SecondaryTelephone;
			PrimaryColor = client.PrimaryColor;
			SecondaryColor = client.SecondaryColor;
			PhoneNumberLength = client.PhoneNumberLength;
			TelephoneCode = client.TelephoneCode;
			DisplayBuyingPrice = client.DisplayBuyingPrice;
			IsEditMode = true;
		}

		[Required]
		public string Name { get; set; }
		public string Slogan { get; set; }
		[Display(Name = "Currency")]
		[Required]
		public string CurrencyFull { get; set; }
		[Display(Name = "Currency Initials")]
		[Required]
		public string CurrencyShort { get; set; }
		[Display(Name = "Country")]
		[Required]
		public string CountryFull { get; set; }
		[Display(Name = "Country Initials")]
		[Required]
		public string CountryShort { get; set; }
		public string Vision { get; set; }
		public string Mission { get; set; }
		public string LogoPath { get; set; }
		[Display(Name = "Enforce Password Policy")]
		public bool EnforcePassword { get; set; }
		[Display(Name = "Password Expiry Months")]
		public int PasswordExpiryMonths { get; set; }
		[Display(Name = "Postal Address")]
		public string PostalAddress { get; set; }
		public string Town { get; set; }
		[Display(Name = "Email Address")]
		public string EmailAddress { get; set; }
		[Display(Name = "Primary Telephone")]
		public string PrimaryTelephone { get; set; }
		[Display(Name = "Secondary Telephone")]
		public string SecondaryTelephone { get; set; }
		[Display(Name = "Primary Color")]
		public string PrimaryColor { get; set; }
		[Display(Name = "Secondary Color")]
		public string SecondaryColor { get; set; }
		[Display(Name = "Phone Number Length")]
		public int PhoneNumberLength { get; set; }
		[Display(Name = "Telephone Code")]
		public string TelephoneCode { get; set; }
		[Display(Name = "Display Product Buying Price")]
		public bool DisplayBuyingPrice { get; set; }
		public bool IsNewImage { get; set; }
		public string NewImagePath { get; set; }
	}
}
