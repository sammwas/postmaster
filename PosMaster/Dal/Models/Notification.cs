namespace PosMaster.Dal
{
	public class Notification : BaseEntity
	{
		public string UserId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public bool IsRead { get; set; }
	}
}
