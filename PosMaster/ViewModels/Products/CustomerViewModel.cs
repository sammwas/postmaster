using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        public CustomerViewModel()
        {

        }

        public CustomerViewModel(Customer customer)
        {
            Id = customer.Id;
            ClientId = customer.ClientId;
            InstanceId = customer.InstanceId;
            Code = customer.Code;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            PhoneNumber = customer.PhoneNumber;
            PostalAddress = customer.PostalAddress;
            Town = customer.Town;
            Location = customer.Location;
            EmailAddress = customer.EmailAddress;
            Website = customer.Website;
            Status = customer.Status;
            IsEditMode = true;
            CreditLimit = customer.CreditLimit;
            Gender = customer.Gender;
            PinNo = customer.PinNo;
        }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Postal Address")]
        public string PostalAddress { get; set; }
        public string Town { get; set; }
        public string Location { get; set; }
        [Display(Name = "Email Address"), DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        public string Gender { get; set; }
        [Display(Name = "Credit Limit")]
        public decimal CreditLimit { get; set; }
        [Display(Name = "KRA PIN No")]
        public string PinNo { get; set; }

    }
}
