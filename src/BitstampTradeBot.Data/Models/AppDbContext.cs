using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BitstampTradeBot.Data.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<MinMaxLog> MinMaxLogs { get; set; }
        public DbSet<Order> Orders { get; set; }

        public AppDbContext() : base("name=DefaultConnection")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MinMaxLog>().Property(p => p.Minimum).HasPrecision(18, 8); 
            modelBuilder.Entity<MinMaxLog>().Property(p => p.Maximum).HasPrecision(18, 8); 
            modelBuilder.Entity<Order>().Property(p => p.BuyAmount).HasPrecision(18, 8); 
            modelBuilder.Entity<Order>().Property(p => p.BuyPrice).HasPrecision(18, 8); 
            modelBuilder.Entity<Order>().Property(p => p.SellAmount).HasPrecision(18, 8); 
            modelBuilder.Entity<Order>().Property(p => p.SellPrice).HasPrecision(18, 8); 
            modelBuilder.Entity<Order>().Property(p => p.BuyFee).HasPrecision(8, 2); 
            modelBuilder.Entity<Order>().Property(p => p.SellFee).HasPrecision(8, 2);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
