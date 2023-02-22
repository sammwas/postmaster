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
            Text = $"{customer.Code} - {customer.FullName} : {customer.PhoneNumber} : {customer.IdNumber}";
            Code = customer.Code;
            PinNo = customer.PinNo;
        }
        public FormSelectViewModel(Supplier supplier)
        {
            Id = supplier.Id.ToString();
            Text = $"{supplier.Code} - {supplier.Name} : {supplier.PrimaryTelephone}";
            Code = supplier.Code;
        }
        public string Id { get; set; }
        public string Text { get; set; }
        public string Code { get; set; }
        public string PinNo { get; set; }
    }
}
