namespace PosMaster.Dal
{
	public class Supplier : BaseEntity
	{
		public string Name { get; set; }
		public string PrimaryTelephone { get; set; }
		public string SecondaryTelephone { get; set; }
		public string PostalAddress { get; set; }
		public string Town { get; set; }
		public string Location { get; set; }
		public string EmailAddress { get; set; }
		public string Website { get; set; }
	}
}
