using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TukangSayurOnline.Api.Data.Entities;

public enum UserRole
{
    Admin,
    TukangSayur,
    Pelanggan
}

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TukangSayurProfile? TukangSayurProfile { get; set; }
}

public class TukangSayurProfile
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string ShopName { get; set; } = string.Empty;

    public decimal Balance { get; set; } = 0m;
    public bool IsOnline { get; set; } = true;

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string CurrentLocationName { get; set; } = string.Empty;

    public List<TukangSayurStock> Stocks { get; set; } = new();
    public List<StockTransaction> Transactions { get; set; } = new();
}

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // e.g. Sayuran Hijau, Buah, Bumbu, Lauk

    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = "kg"; // e.g. kg, ikat, pack, gram

    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal DefaultPrice { get; set; } = 0m;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TukangSayurStock
{
    [Key]
    public int Id { get; set; }

    public int TukangSayurId { get; set; }
    [ForeignKey("TukangSayurId")]
    public TukangSayurProfile TukangSayur { get; set; } = null!;

    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    public double StockQuantity { get; set; } = 0;
    public decimal PricePerUnit { get; set; } = 0m;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum StockTransactionType
{
    RestockIn, // Tukang sayur beli barang untuk nyetok (Masuk)
    SaleOut,   // Barang terjual ke pelanggan (Keluar)
    ManualAdjustment
}

public class StockTransaction
{
    [Key]
    public int Id { get; set; }

    public int TukangSayurId { get; set; }
    [ForeignKey("TukangSayurId")]
    public TukangSayurProfile TukangSayur { get; set; } = null!;

    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    public StockTransactionType Type { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }

    public string Notes { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}

public enum OrderStatus
{
    Pending,
    Completed,
    Cancelled
}

public class Order
{
    [Key]
    public int Id { get; set; }

    public int CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public User Customer { get; set; } = null!;

    public int TukangSayurId { get; set; }
    [ForeignKey("TukangSayurId")]
    public TukangSayurProfile TukangSayur { get; set; } = null!;

    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Completed;

    public double MeetLatitude { get; set; }
    public double MeetLongitude { get; set; }
    public string MeetAddress { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }
    [ForeignKey("OrderId")]
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}
