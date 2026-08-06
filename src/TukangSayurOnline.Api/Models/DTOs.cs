using System;
using System.Collections.Generic;
using TukangSayurOnline.Api.Data.Entities;

namespace TukangSayurOnline.Api.Models;

public record RegisterRequest(
    string FullName,
    string Email,
    string Phone,
    string Password,
    UserRole Role,
    string ShopName, // If TukangSayur
    string Address,
    double Latitude,
    double Longitude
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    bool Success,
    string Message,
    string Token,
    int UserId,
    string FullName,
    string Email,
    string Role,
    int? TukangSayurId
);

public record ProductDto(
    int Id,
    string Name,
    string Category,
    string Unit,
    string Description,
    string ImageUrl,
    decimal DefaultPrice
);

public record CreateProductRequest(
    string Name,
    string Category,
    string Unit,
    string Description,
    string ImageUrl,
    decimal DefaultPrice
);

public record UpdateProductRequest(
    int Id,
    string Name,
    string Category,
    string Unit,
    string Description,
    string ImageUrl,
    decimal DefaultPrice
);

public record UpdateStockRequest(
    int ProductId,
    double StockQuantity,
    decimal PricePerUnit
);

public record RestockInRequest(
    int ProductId,
    double Quantity,
    decimal UnitPrice, // Harga beli per unit
    string Notes
);

public record RecordSaleRequest(
    int ProductId,
    double Quantity,
    decimal UnitPrice, // Harga jual per unit
    string Notes
);

public record StockItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string Category,
    string Unit,
    string ImageUrl,
    double StockQuantity,
    decimal PricePerUnit,
    DateTime UpdatedAt
);

public record NearbyVendorProductDto(
    int TukangSayurId,
    string ShopName,
    string VendorName,
    string VendorPhone,
    double VendorLatitude,
    double VendorLongitude,
    double DistanceKm,
    bool IsOnline,
    int ProductId,
    string ProductName,
    string Category,
    string Unit,
    string ImageUrl,
    double AvailableStock,
    decimal PricePerUnit
);

public record CreateOrderRequest(
    int TukangSayurId,
    double MeetLatitude,
    double MeetLongitude,
    string MeetAddress,
    List<OrderItemRequest> Items
);

public record OrderItemRequest(
    int ProductId,
    double Quantity,
    decimal UnitPrice
);

public record OrderDto(
    int Id,
    int CustomerId,
    string CustomerName,
    int TukangSayurId,
    string ShopName,
    decimal TotalAmount,
    string Status,
    double MeetLatitude,
    double MeetLongitude,
    string MeetAddress,
    DateTime OrderDate,
    List<OrderItemDto> Items
);

public record OrderItemDto(
    int ProductId,
    string ProductName,
    string Unit,
    double Quantity,
    decimal UnitPrice,
    decimal SubTotal
);

public record StockTransactionDto(
    int Id,
    int ProductId,
    string ProductName,
    string Type,
    double Quantity,
    decimal UnitPrice,
    decimal TotalAmount,
    string Notes,
    DateTime TransactionDate
);

public record IncomeSummaryDto(
    decimal CurrentBalance,
    decimal TotalIncomeAllTime,
    decimal TotalRestockExpenseAllTime,
    int TotalSalesCount,
    List<StockTransactionDto> RecentTransactions
);

public record PopularProductReportDto(
    int ProductId,
    string ProductName,
    string Category,
    double TotalQuantitySold,
    decimal TotalRevenue
);

public record EmptyStockReportDto(
    int TukangSayurId,
    string ShopName,
    string VendorName,
    int ProductId,
    string ProductName,
    string Category
);
