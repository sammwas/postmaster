using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PosMaster.Dal
{
	public class DatabaseContext : IdentityDbContext<User>
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		{

		}
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
	}
}
