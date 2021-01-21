namespace PosMaster.Dal.Interfaces
{
	public class  ReturnData <T>
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public string ErrorMessage { get; set; }
		public T Data { get; set; }
	}
}
