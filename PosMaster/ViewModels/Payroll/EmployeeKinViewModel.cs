using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class EmployeeKinViewModel : BaseViewModel
	{
		public EmployeeKinViewModel()
		{

		}

		public EmployeeKinViewModel(EmployeeKin kin)
		{
			IsEditMode = true;
			Id = kin.Id;
			ClientId = kin.ClientId;
			InstanceId = kin.InstanceId;
			Code = kin.Code;
			Status = kin.Status;
			Notes = kin.Notes;
			UserId = kin.UserId;
			FirstName = kin.FirstName;
			MiddleName = kin.MiddleName;
			LastName = kin.LastName;
			Gender = kin.Gender;
			Title = kin.Title;
			PostalAddress = kin.PostalAddress;
			Town = kin.Town;
			EmailAddress = kin.EmailAddress;
			PhoneNumber = kin.PhoneNumber;
			AltPhoneNumber = kin.AltPhoneNumber;
			Relationship = kin.Relationship;
		}
		public string UserId { get; set; }
		[Display(Name = "First Name")]
		public string FirstName { get; set; }
		[Display(Name = "Middle Name")]
		public string MiddleName { get; set; }
		[Display(Name = "Last Name")]
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string Title { get; set; }
		[Display(Name = "Postal Address")]
		public string PostalAddress { get; set; }
		public string Town { get; set; }
		[Display(Name = "Email Address")]
		[DataType(DataType.EmailAddress)]
		public string EmailAddress { get; set; }
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }
		[Display(Name = "Alt. Phone Number")]
		public string AltPhoneNumber { get; set; }
		public string Relationship { get; set; }
	}
}
