using PosMaster.Dal;

namespace PosMaster.ViewModels
{
	public class FormSelectViewModel
	{
		public FormSelectViewModel()
		{

		}

		public FormSelectViewModel(Customer customer)
		{
			Id = customer.Id.ToString();
			Text = $"{customer.Code} {customer.FullName} : {customer.PhoneNumber} : {customer.IdNumber}";
		}

		public string Id { get; set; }
		public string Text { get; set; }
	}
}
