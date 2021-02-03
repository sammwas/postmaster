using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class UploadImageViewModel
	{
		[Required]
		public IFormFile File { get; set; }
		public string UserId { get; set; }
		public string CurrentImage { get; set; }
	}
}
