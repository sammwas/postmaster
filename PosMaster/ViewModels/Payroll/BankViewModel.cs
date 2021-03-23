using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class BankViewModel : BaseViewModel
	{
		public BankViewModel()
		{

		}

		public BankViewModel(Bank bank)
		{
			IsEditMode = true;
			Name = bank.Name;
			Notes = bank.Notes;
			ContactPerson = bank.ContactPerson;
			PhoneNumber = bank.PhoneNumber;
			EmailAddress = bank.EmailAddress;
			Website = bank.Website;
			Id = bank.Id;
			ClientId = bank.ClientId;
			InstanceId = bank.InstanceId;
			Code = bank.Code;
			Status = bank.Status;
		}

		public string Name { get; set; }
		[Display(Name = "Contact Person")]
		public string ContactPerson { get; set; }
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }
		[Display(Name = "Email Address")]
		[DataType(DataType.EmailAddress)]
		public string EmailAddress { get; set; }
		[DataType(DataType.Url)]
		public string Website { get; set; }
	}
}
