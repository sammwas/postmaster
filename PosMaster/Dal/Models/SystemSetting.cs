namespace PosMaster.Dal
{
	public class SystemSetting : BaseEntity
	{
		public string Name { get; set; }
		public string Tagline { get; set; }
		public string Description { get; set; }
		public string Version { get; set; }
		public string PhoneNumber { get; set; }
		public string EmailAddress { get; set; }
		public string PostalAddress { get; set; }
		public string Town { get; set; }
		public string LogoPath { get; set; }
		public string Privacy { get; set; }
		public string TermsAndConditions { get; set; }
	}
}
