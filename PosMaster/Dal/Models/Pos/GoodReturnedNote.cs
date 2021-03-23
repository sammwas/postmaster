using System;

namespace PosMaster.Dal
{
	public class GoodReturnedNote : BaseEntity
	{
		public Guid ReceiptId { get; set; }
		public Receipt Receipt { get; set; }
	}
}
