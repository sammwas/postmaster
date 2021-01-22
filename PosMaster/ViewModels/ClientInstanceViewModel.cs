using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class ClientInstanceViewModel : BaseViewModel
	{
		public ClientInstanceViewModel()
		{

		}

		public ClientInstanceViewModel(ClientInstance instance)
		{
			Id = instance.Id;
			OpeningTime = $"{string.Format("{0:hh:mm tt}", instance.OpeningTime)}";
			ClosingTime = $"{string.Format("{0:hh:mm tt}", instance.ClosingTime)}";
			Latitude = instance.Latitude;
			Longitude = instance.Longitude;
			Name = instance.Name;
			PostalAddress = instance.PostalAddress;
			Town = instance.Town;
			Location = instance.Location;
			PrimaryTelephone = instance.PrimaryTelephone;
			SecondaryTelephone = instance.SecondaryTelephone;
			Notes = instance.Notes;
			ClientId = instance.ClientId;
			InstanceId = instance.InstanceId;
			IsEditMode = true;
		}

		[Display(Name = "Opening Time")]
		public string OpeningTime { get; set; }
		[Display(Name = "Closing Time")]
		public string ClosingTime { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		[Required]
		public string Name { get; set; }
		[Display(Name = "Postal Address")]
		public string PostalAddress { get; set; }
		public string Town { get; set; }
		public string Location { get; set; }
		[Display(Name = "Primary Telephone")]
		public string PrimaryTelephone { get; set; }
		[Display(Name = "Secondary Telephone")]
		public string SecondaryTelephone { get; set; }
	}
}
