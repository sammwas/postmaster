using PosMaster.Dal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PosMaster.ViewModels
{
	public class OrderViewModel : BaseViewModel
	{
        public OrderViewModel()
        {

        }
        public OrderViewModel(Order order)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            Id = order.Id;
			Name = order.Name;
			CustomerId = order.CustomerId.ToString();
			LineItemListStr = JsonConvert.SerializeObject(order.OrderLineItems, serializerSettings);
			IsEditMode = true;
			ClientId = order.ClientId;
			InstanceId = order.InstanceId;
			Notes = order.Notes;
			Status = order.Status;
			IsEditMode = true;
			Code = order.Code;
		}
		[Required]
		[Display(Name = "Title")]
		public string Name { get; set; }
		[Required]
		[Display(Name = "Customer")]
		public string CustomerId { get; set; }
		public string LineItemListStr { get; set; }
	}
}
