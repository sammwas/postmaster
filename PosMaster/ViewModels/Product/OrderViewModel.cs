using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.ViewModels.Product
{
    public class OrderViewModel : BaseViewModel
    {
		[Required]
		[Display(Name = "Title")]
		public string Name { get; set; }
		[Required]
		[Display(Name = "Customer")]
		public string CustomerId { get; set; }
		public string LineItemListStr { get; set; }
	}
}
