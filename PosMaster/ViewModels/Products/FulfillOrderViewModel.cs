using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.ViewModels
{
    public class FulfillOrderViewModel : BaseViewModel
    {
        public string PaymentModeIdStr { get; set; }
        public string PaymentModeNo { get; set; }
        public string PersonnelName { get; set; }
    }
}