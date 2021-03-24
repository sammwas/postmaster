using PosMaster.Dal;
using System;

namespace PosMaster.ViewModels
{
	public class MonthlyPayViewModel
	{
		public MonthlyPayViewModel()
		{

		}

		public MonthlyPayViewModel(EmployeeMonthPayment payment)
		{
			Id = payment.Id;
			User = payment.User;
			BasicPay = payment.BasicPay;
			Deduction = payment.Deduction;
			Allowance = payment.Allowance;
			Year = payment.Year;
			Month = payment.Month;
			Approved = true;
			DateApproved = payment.DateApproved;
		}

		public MonthlyPayViewModel(EmployeeSalary salary)
		{
			Id = salary.Id;
			User = salary.User;
			BasicPay = salary.BasicPay;
			Deduction = salary.Deduction;
			Allowance = salary.Allowance;
		}
		public Guid Id { get; set; }
		public User User { get; set; }
		public decimal BasicPay { get; set; }
		public decimal Deduction { get; set; }
		public decimal Allowance { get; set; }
		public int Year { get; set; }
		public int Month { get; set; }
		public bool Approved { get; set; }
		public DateTime? DateApproved { get; set; }
		public decimal NetAmount => BasicPay + Allowance - Deduction;
	}
}
