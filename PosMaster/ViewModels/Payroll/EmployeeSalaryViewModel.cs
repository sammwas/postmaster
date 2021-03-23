using Microsoft.AspNetCore.Mvc;
using PosMaster.Dal;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
	public class EmployeeSalaryViewModel : BaseViewModel
	{
		public EmployeeSalaryViewModel()
		{

		}

		public EmployeeSalaryViewModel(EmployeeSalary salary)
		{
			IsEditMode = true;
			Id = salary.Id;
			ClientId = salary.ClientId;
			InstanceId = salary.InstanceId;
			Code = salary.Code;
			Status = salary.Status;
			Notes = salary.Notes;
			UserId = salary.UserId;
			Bank = salary.Bank;
			BankAccount = salary.BankAccount;
			BasicPay = salary.BasicPay;
			Deduction = salary.Deduction;
			Allowance = salary.Allowance;
		}
		[Required]
		[HiddenInput]
		[Display(Name = "Employee")]
		public string UserId { get; set; }
		public string Bank { get; set; }
		[Display(Name = "Bank Acc.")]
		public string BankAccount { get; set; }
		[Display(Name = "Basic Pay")]
		public decimal BasicPay { get; set; }
		public decimal Allowance { get; set; }
		public decimal Deduction { get; set; }
	}
}
