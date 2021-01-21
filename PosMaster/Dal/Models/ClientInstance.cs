using System;

namespace PosMaster.Dal
{
	public class ClientInstance : BaseEntity
	{
		public Client Client { get; set; }
		public DateTime OpeningTime { get; set; }
		public DateTime ClosingTime { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public string Name { get; set; }
		public string PostalAddress { get; set; }
		public string Town { get; set; }
		public string Location { get; set; }
		public string PrimaryTelephone { get; set; }
		public string SecondaryTelephone { get; set; }
	}
}
