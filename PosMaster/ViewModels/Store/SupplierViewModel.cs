using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class SupplierViewModel : BaseViewModel
	{
		public SupplierViewModel()
		{

		}

		public SupplierViewModel(Supplier supplier)
		{
			Id = supplier.Id;
			ClientId = supplier.ClientId;
			InstanceId = supplier.InstanceId;
			Code = supplier.Code;
			Name = supplier.Name;
			PrimaryTelephone = supplier.PrimaryTelephone;
			SecondaryTelephone = supplier.SecondaryTelephone;
			PostalAddress = supplier.PostalAddress;
			Town = supplier.Town;
			Location = supplier.Location;
			EmailAddress = supplier.EmailAddress;
			Website = supplier.Website;
			Status = supplier.Status;
			IsEditMode = true;
		}

		[Required]
		public string Name { get; set; }
		[Display(Name = "Primary Telephone")]
		public string PrimaryTelephone { get; set; }
		[Display(Name = "Secondary Telephone")]
		public string SecondaryTelephone { get; set; }
		[Display(Name = "Postal Address")]
		public string PostalAddress { get; set; }
		public string Town { get; set; }
		public string Location { get; set; }
		[Display(Name = "Email Address")]
		public string EmailAddress { get; set; }
		public string Website { get; set; }
	}
}
