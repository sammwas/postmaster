namespace PosMaster.Dal
{
	public class Bank : BaseEntity
	{
		public string Name { get; set; }
		public string ContactPerson { get; set; }
		public string PhoneNumber { get; set; }
		public string EmailAddress { get; set; }
		public string Website { get; set; }
	}
}
