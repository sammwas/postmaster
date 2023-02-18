namespace PosMaster.Dal
{
	public class Client : BaseEntity
	{
		public string Name { get; set; }
		public string Slogan { get; set; }
		public string CurrencyFull { get; set; }
		public string CurrencyShort { get; set; }
		public string CountryFull { get; set; }
		public string CountryShort { get; set; }
		public string Vision { get; set; }
		public string Mission { get; set; }
		public string LogoPath { get; set; }
		public bool EnforcePassword { get; set; }
		public int PasswordExpiryMonths { get; set; }
		public string PostalAddress { get; set; }
		public string Town { get; set; }
		public string EmailAddress { get; set; }
		public string PrimaryTelephone { get; set; }
		public string SecondaryTelephone { get; set; }
		public string PrimaryColor { get; set; }
		public string SecondaryColor { get; set; }
		public int PhoneNumberLength { get; set; }
		public string TelephoneCode { get; set; }
		public bool DisplayBuyingPrice { get; set; }
	}
}
