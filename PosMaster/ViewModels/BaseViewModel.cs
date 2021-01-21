using PosMaster.Dal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class BaseViewModel
	{
		public string ErrorMessage { get; set; }
		public string Message { get; set; }
		public string Notes { get; set; }
		public bool Success { get; set; }
		public bool IsEditMode { get; set; }
		public EntityStatus Status { get; set; }
		public string Personnel { get; set; }
		[HiddenInput]
		public Guid Id { get; set; }
		[Required]
		public string Code { get; set; }
	}
}
