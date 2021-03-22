namespace PosMaster.Dal
{
	public class EmployeeKin : BaseEntity
	{
		public User User { get; set; }
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string Title { get; set; }
		public string PostalAddress { get; set; }
		public string Town { get; set; }
		public string EmailAddress { get; set; }
		public string PhoneNumber { get; set; }
		public string AltPhoneNumber { get; set; }
	}
}
