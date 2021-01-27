using PosMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PosMaster.Dal.Interfaces
{
public	interface ISupplierInterface
	{
		Task<ReturnData<Product>> EditAsync(ProductViewModel model);
		Task<ReturnData<List<Product>>> AllAsync();
		Task<ReturnData<List<Product>>> ByClientIdAsync(Guid clientId); 
		Task<ReturnData<Product>> ByIdAsync(Guid id);
	}
}
