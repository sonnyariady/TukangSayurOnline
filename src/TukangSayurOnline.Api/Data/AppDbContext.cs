using Microsoft.EntityFrameworkCore;
using TukangSayurOnline.Api.Data.Entities;

namespace TukangSayurOnline.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TukangSayurProfile> TukangSayurProfiles => Set<TukangSayurProfile>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<TukangSayurStock> TukangSayurStocks => Set<TukangSayurStock>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<TukangSayurProfile>()
            .HasOne(p => p.User)
            .WithOne(u => u.TukangSayurProfile)
            .HasForeignKey<TukangSayurProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TukangSayurStock>()
            .HasOne(s => s.TukangSayur)
            .WithMany(p => p.Stocks)
            .HasForeignKey(s => s.TukangSayurId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TukangSayurStock>()
            .HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockTransaction>()
            .HasOne(t => t.TukangSayur)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.TukangSayurId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StockTransaction>()
            .HasOne(t => t.Product)
            .WithMany()
            .HasForeignKey(t => t.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
