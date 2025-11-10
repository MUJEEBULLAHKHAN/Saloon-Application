using Microsoft.EntityFrameworkCore;
using SaloonWebApi.Models;

namespace SaloonWebApi.Data
{
    public class SaloonDbContext : DbContext
    {
        public SaloonDbContext(DbContextOptions<SaloonDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Deal> Deals { get; set; } = null!;
        public DbSet<DealService> DealServices { get; set; } = null!;
        public DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
        public DbSet<DealCategory> DealCategories { get; set; } = null!;
        public DbSet<BookingMaster> BookingMasters { get; set; } = null!;
        public DbSet<BookingDetail> BookingDetails { get; set; } = null!;
        public DbSet<BookingStatus> BookingStatuses { get; set; } = null!;
        public DbSet<BookingServiceStatus> BookingServiceStatuses { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;

        // New product-related entities
        public DbSet<ProductCat> ProductCats { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductSaleMaster> ProductSaleMasters { get; set; } = null!;
        public DbSet<ProductSaleDetail> ProductSaleDetails { get; set; } = null!;

        // Service sale entities
        public DbSet<ServiceSaleMaster> ServiceSaleMasters { get; set; } = null!;
        public DbSet<ServiceSaleDetail> ServiceSaleDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map table names and keys to match existing DB
            modelBuilder.Entity<Customer>().ToTable("Customers").HasKey(c => c.Customer_Id);
            modelBuilder.Entity<Employee>().ToTable("Employees").HasKey(e => e.Employee_Id);
            modelBuilder.Entity<Service>().ToTable("Services").HasKey(s => s.Service_Id);
            modelBuilder.Entity<Deal>().ToTable("Deal").HasKey(d => d.Deal_Id);
            modelBuilder.Entity<DealService>().ToTable("Deal_Services").HasKey(ds => ds.Deal_Service_Id);
            modelBuilder.Entity<ServiceCategory>().ToTable("ServiceCat").HasKey(sc => sc.Category_Id);
            modelBuilder.Entity<DealCategory>().ToTable("Deal_Category").HasKey(dc => dc.Id);
            modelBuilder.Entity<BookingMaster>().ToTable("Booking_Master").HasKey(b => b.Id);
            modelBuilder.Entity<BookingDetail>().ToTable("Booking_Detail").HasKey(bd => bd.Id);
            modelBuilder.Entity<BookingStatus>().ToTable("Booking_Status").HasKey(bs => bs.Id);
            modelBuilder.Entity<BookingServiceStatus>().ToTable("Booking_Service_Status").HasKey(bss => bss.Id);
            modelBuilder.Entity<Role>().ToTable("Roles").HasKey(r => r.Role_Id);
            modelBuilder.Entity<User>().ToTable("Users").HasKey(u => u.User_Id);
            modelBuilder.Entity<UserRole>().ToTable("UserRoles").HasKey(ur => ur.UserRole_Id);
            modelBuilder.Entity<Branch>().ToTable("Branch").HasKey(br => br.Id);

            // product related mappings
            modelBuilder.Entity<ProductCat>().ToTable("ProductCat").HasKey(pc => pc.Category_Id);
            modelBuilder.Entity<Product>().ToTable("Products").HasKey(p => p.Product_Id);
            modelBuilder.Entity<ProductSaleMaster>().ToTable("ProductSale_Master").HasKey(pm => pm.Sale_Id);
            modelBuilder.Entity<ProductSaleDetail>().ToTable("ProductSale_Detail").HasKey(pd => pd.SaleDetail_Id);

            // service sale mappings
            modelBuilder.Entity<ServiceSaleMaster>().ToTable("ServiceSale_Master").HasKey(sm => sm.Service_Id);
            modelBuilder.Entity<ServiceSaleDetail>().ToTable("ServiceSale_Detail").HasKey(sd => sd.Service_Detail_Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}