using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PosMaster.Dal
{
	public class DatabaseContext : IdentityDbContext<User>
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		{

		}
		public DbSet<SystemSetting> SystemSettings { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<ClientInstance> ClientInstances { get; set; }
		public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductCategory> ProductCategories { get; set; }
		public DbSet<ProductPriceLog> ProductPriceLogs { get; set; }
		public DbSet<Receipt> Receipts { get; set; }
		public DbSet<Supplier> Suppliers { get; set; }
		public DbSet<ProductStockAdjustmentLog> ProductStockAdjustmentLogs { get; set; }
		public DbSet<ExpenseType> ExpenseTypes { get; set; }
		public DbSet<Expense> Expenses { get; set; }
		public DbSet<PaymentMode> PaymentModes { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<UserLoginLog> UserLoginLogs { get; set; }
		public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
		public DbSet<GoodReceivedNote> GoodReceivedNotes { get; set; }
		public DbSet<PoGrnProduct> PoGrnProducts { get; set; }
		public DbSet<EmailSetting> EmailSettings { get; set; }
		public DbSet<SmsSetting> SmsSettings { get; set; }
		public DbSet<Notification> Notifications { get; set; }
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<ReceiptLineItem> ReceiptLineItems { get; set; }
		public DbSet<GoodReturnedNote> GoodReturnedNotes { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderLineItem> OrderLineItems { get; set; }
		public DbSet<Bank> Banks { get; set; }
		public DbSet<County> Counties { get; set; }
		public DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
		public DbSet<EmployeeMonthPayment> EmployeeMonthPayments { get; set; }
		public DbSet<EmployeeKin> EmployeeKins { get; set; }
		public DbSet<EmployeeLeaveApplication> EmployeeLeaveApplications { get; set; }
		public DbSet<EmployeeLeaveCategory> EmployeeLeaveCategories { get; set; }
		public DbSet<EmployeeLeaveEntitlement> EmployeeLeaveEntitlements { get; set; }
		public DbSet<EmployeeSalaryLog> EmployeeSalaryLogs { get; set; }
	}
}
