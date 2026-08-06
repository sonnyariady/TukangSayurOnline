using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TukangSayurOnline.Api.Data.Entities;

namespace TukangSayurOnline.Api.Data;

public static class DbInitializer
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password + "TS_SALT_2026");
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (!await context.Users.AnyAsync())
        {
            // 1. Create Admin
            var admin = new User
            {
                FullName = "Administrator Sayur",
                Email = "admin@tukangsayur.com",
                Phone = "081234567890",
                PasswordHash = HashPassword("Admin123!"),
                Role = UserRole.Admin,
                Address = "Kantor Pusat Tukang Sayur Online, Jakarta",
                Latitude = -6.2088,
                Longitude = 106.8456
            };
            context.Users.Add(admin);

            // 2. Create Products
            var products = new List<Product>
            {
                new Product { Name = "Bayam Hijau Segar", Category = "Sayuran Hijau", Unit = "ikat", Description = "Bayam petik baru segar kaya zat besi", ImageUrl = "https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=400", DefaultPrice = 3500 },
                new Product { Name = "Kangkung Darat", Category = "Sayuran Hijau", Unit = "ikat", Description = "Kangkung segar daun muda", ImageUrl = "https://images.unsplash.com/photo-1540420773420-3366772f4999?w=400", DefaultPrice = 3000 },
                new Product { Name = "Wortel Manis Dieng", Category = "Sayuran Umbi", Unit = "kg", Description = "Wortel Dieng segar manis renyah", ImageUrl = "https://images.unsplash.com/photo-1598170845058-12ef4a457939?w=400", DefaultPrice = 12000 },
                new Product { Name = "Tomat Buah Merah", Category = "Sayuran Buah", Unit = "kg", Description = "Tomat merah segar cocok untuk masakan & jus", ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=400", DefaultPrice = 14000 },
                new Product { Name = "Cabai Rawit Merah", Category = "Bumbu & Rempah", Unit = "kg", Description = "Cabai rawit super pedas segar", ImageUrl = "https://images.unsplash.com/photo-1588252303782-cb80119abd6d?w=400", DefaultPrice = 45000 },
                new Product { Name = "Bawang Merah Brebes", Category = "Bumbu & Rempah", Unit = "kg", Description = "Bawang merah olahan Brebes harum", ImageUrl = "https://images.unsplash.com/photo-1618512496248-a07fe83aa8cf?w=400", DefaultPrice = 38000 },
                new Product { Name = "Kentang Dieng Super", Category = "Sayuran Umbi", Unit = "kg", Description = "Kentang Dieng besar mulus", ImageUrl = "https://images.unsplash.com/photo-1518977676601-b53f82aba655?w=400", DefaultPrice = 18000 },
                new Product { Name = "Tempe Organik Super", Category = "Lauk Pauk", Unit = "papan", Description = "Tempe kedelai padat segar alami", ImageUrl = "https://images.unsplash.com/photo-1628102491629-778571d893a3?w=400", DefaultPrice = 6000 },
                new Product { Name = "Tahu Putih Halus", Category = "Lauk Pauk", Unit = "bungkus", Description = "Tahu putih segar lembut 10 pcs", ImageUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400", DefaultPrice = 7000 },
                new Product { Name = "Daging Ayam Broiler Segar", Category = "Daging & Ikan", Unit = "kg", Description = "Daging ayam utuh segar potong", ImageUrl = "https://images.unsplash.com/photo-1604503468506-a8da13d82791?w=400", DefaultPrice = 36000 }
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // 3. Create Tukang Sayur 1: Mang Udin
            var userUdin = new User
            {
                FullName = "Mang Udin Sutarman",
                Email = "mang.udin@gmail.com",
                Phone = "081987654321",
                PasswordHash = HashPassword("Udin123!"),
                Role = UserRole.TukangSayur,
                Address = "Jl. Kelapa Gading No. 12, Jakarta Utara",
                Latitude = -6.1550,
                Longitude = 106.9020
            };
            context.Users.Add(userUdin);
            await context.SaveChangesAsync();

            var profileUdin = new TukangSayurProfile
            {
                UserId = userUdin.Id,
                ShopName = "Sayur Segar Mang Udin",
                Balance = 350000m,
                IsOnline = true,
                Latitude = -6.1550,
                Longitude = 106.9020,
                CurrentLocationName = "Kelapa Gading Permai"
            };
            context.TukangSayurProfiles.Add(profileUdin);

            // 4. Create Tukang Sayur 2: Bang Budi
            var userBudi = new User
            {
                FullName = "Bang Budi Santoso",
                Email = "bang.budi@gmail.com",
                Phone = "081777888999",
                PasswordHash = HashPassword("Budi123!"),
                Role = UserRole.TukangSayur,
                Address = "Jl. Tebet Raya No. 45, Jakarta Selatan",
                Latitude = -6.2250,
                Longitude = 106.8550
            };
            context.Users.Add(userBudi);
            await context.SaveChangesAsync();

            var profileBudi = new TukangSayurProfile
            {
                UserId = userBudi.Id,
                ShopName = "Lapak Sayur Bang Budi",
                Balance = 520000m,
                IsOnline = true,
                Latitude = -6.2250,
                Longitude = 106.8550,
                CurrentLocationName = "Tebet Eco Park"
            };
            context.TukangSayurProfiles.Add(profileBudi);

            // 5. Create Pelanggan
            var userPelanggan = new User
            {
                FullName = "Ibu Siti Aminah",
                Email = "pelanggan@gmail.com",
                Phone = "085611223344",
                PasswordHash = HashPassword("Pelanggan123!"),
                Role = UserRole.Pelanggan,
                Address = "Jl. Sunter Garden No. 8, Jakarta Utara",
                Latitude = -6.1480,
                Longitude = 106.8720
            };
            context.Users.Add(userPelanggan);
            await context.SaveChangesAsync();

            // 6. Add initial stocks & restock transactions for Mang Udin
            var udinStocks = new List<TukangSayurStock>
            {
                new TukangSayurStock { TukangSayurId = profileUdin.Id, ProductId = products[0].Id, StockQuantity = 20, PricePerUnit = 4000 },
                new TukangSayurStock { TukangSayurId = profileUdin.Id, ProductId = products[1].Id, StockQuantity = 15, PricePerUnit = 3500 },
                new TukangSayurStock { TukangSayurId = profileUdin.Id, ProductId = products[2].Id, StockQuantity = 10, PricePerUnit = 13000 },
                new TukangSayurStock { TukangSayurId = profileUdin.Id, ProductId = products[3].Id, StockQuantity = 8, PricePerUnit = 15000 },
                new TukangSayurStock { TukangSayurId = profileUdin.Id, ProductId = products[4].Id, StockQuantity = 0, PricePerUnit = 48000 }, // Empty stock
                new TukangSayurStock { TukangSayurId = profileUdin.Id, ProductId = products[7].Id, StockQuantity = 12, PricePerUnit = 6500 }
            };
            context.TukangSayurStocks.AddRange(udinStocks);

            // 7. Add initial stocks for Bang Budi
            var budiStocks = new List<TukangSayurStock>
            {
                new TukangSayurStock { TukangSayurId = profileBudi.Id, ProductId = products[0].Id, StockQuantity = 10, PricePerUnit = 4000 },
                new TukangSayurStock { TukangSayurId = profileBudi.Id, ProductId = products[3].Id, StockQuantity = 15, PricePerUnit = 14500 },
                new TukangSayurStock { TukangSayurId = profileBudi.Id, ProductId = products[4].Id, StockQuantity = 5, PricePerUnit = 47000 },
                new TukangSayurStock { TukangSayurId = profileBudi.Id, ProductId = products[5].Id, StockQuantity = 12, PricePerUnit = 40000 },
                new TukangSayurStock { TukangSayurId = profileBudi.Id, ProductId = products[6].Id, StockQuantity = 0, PricePerUnit = 19000 }, // Empty stock
                new TukangSayurStock { TukangSayurId = profileBudi.Id, ProductId = products[9].Id, StockQuantity = 6, PricePerUnit = 38000 }
            };
            context.TukangSayurStocks.AddRange(budiStocks);

            // 8. Sample transactions
            var trans1 = new StockTransaction
            {
                TukangSayurId = profileUdin.Id,
                ProductId = products[0].Id,
                Type = StockTransactionType.RestockIn,
                Quantity = 30,
                UnitPrice = 3000,
                TotalAmount = 90000,
                Notes = "Pembelian kulakan di Pasar Induk Kramat Jati",
                TransactionDate = DateTime.UtcNow.AddDays(-2)
            };

            var trans2 = new StockTransaction
            {
                TukangSayurId = profileUdin.Id,
                ProductId = products[0].Id,
                Type = StockTransactionType.SaleOut,
                Quantity = 10,
                UnitPrice = 4000,
                TotalAmount = 40000,
                Notes = "Penjualan ke Ibu Siti (Pelanggan)",
                TransactionDate = DateTime.UtcNow.AddDays(-1)
            };

            context.StockTransactions.AddRange(trans1, trans2);

            await context.SaveChangesAsync();
        }
    }
}
