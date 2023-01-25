using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PosMaster.Dal;

namespace PosMaster.ViewModels
{
    public class TaxTypeViewModel : BaseViewModel
    {
        public TaxTypeViewModel()
        {

        }

        public TaxTypeViewModel(TaxType tax)
        {
            Id = tax.Id;
            ClientId = tax.ClientId;
            InstanceId = tax.InstanceId;
            Notes = tax.Notes;
            Status = tax.Status;
            Name = tax.Name;
            IsEditMode = true;
            Code = tax.Code;
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Rate { get; set; }
    }
}