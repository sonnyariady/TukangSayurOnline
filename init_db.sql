-- ============================================================================
-- SCRIPT DATABASE & SEED DATA: TUKANG SAYUR ONLINE
-- Database Target: PostgreSQL (DbTukangSayurOnline)
-- ============================================================================

-- Step 1: Buat Database (Eksekusi terpisah jika database belum ada)
-- CREATE DATABASE "DbTukangSayurOnline";
-- \c DbTukangSayurOnline;

-- ============================================================================
-- 1. DROP TABLE JIKA SUDAH ADA (URUTAN DARI CHILD KE PARENT)
-- ============================================================================
DROP TABLE IF EXISTS "OrderItems" CASCADE;
DROP TABLE IF EXISTS "Orders" CASCADE;
DROP TABLE IF EXISTS "StockTransactions" CASCADE;
DROP TABLE IF EXISTS "TukangSayurStocks" CASCADE;
DROP TABLE IF EXISTS "TukangSayurProfiles" CASCADE;
DROP TABLE IF EXISTS "Products" CASCADE;
DROP TABLE IF EXISTS "Users" CASCADE;

-- ============================================================================
-- 2. CREATE TABLES (STRUKTUR TABEL RELEVAN EF CORE)
-- ============================================================================

-- Tabel Users
CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "FullName" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(100) NOT NULL UNIQUE,
    "Phone" VARCHAR(20) NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Role" INT NOT NULL, -- 0: Admin, 1: TukangSayur, 2: Pelanggan
    "Address" TEXT NOT NULL DEFAULT '',
    "Latitude" DOUBLE PRECISION NOT NULL DEFAULT 0,
    "Longitude" DOUBLE PRECISION NOT NULL DEFAULT 0,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabel TukangSayurProfiles
CREATE TABLE "TukangSayurProfiles" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL UNIQUE REFERENCES "Users"("Id") ON DELETE CASCADE,
    "ShopName" VARCHAR(100) NOT NULL,
    "Balance" NUMERIC(18, 2) NOT NULL DEFAULT 0.00,
    "IsOnline" BOOLEAN NOT NULL DEFAULT TRUE,
    "Latitude" DOUBLE PRECISION NOT NULL DEFAULT 0,
    "Longitude" DOUBLE PRECISION NOT NULL DEFAULT 0,
    "CurrentLocationName" TEXT NOT NULL DEFAULT ''
);

-- Tabel Products (Master Barang)
CREATE TABLE "Products" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Category" VARCHAR(50) NOT NULL,
    "Unit" VARCHAR(20) NOT NULL DEFAULT 'kg',
    "Description" TEXT NOT NULL DEFAULT '',
    "ImageUrl" TEXT NOT NULL DEFAULT '',
    "DefaultPrice" NUMERIC(18, 2) NOT NULL DEFAULT 0.00,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabel TukangSayurStocks (Stok Per Tukang Sayur)
CREATE TABLE "TukangSayurStocks" (
    "Id" SERIAL PRIMARY KEY,
    "TukangSayurId" INT NOT NULL REFERENCES "TukangSayurProfiles"("Id") ON DELETE CASCADE,
    "ProductId" INT NOT NULL REFERENCES "Products"("Id") ON DELETE RESTRICT,
    "StockQuantity" DOUBLE PRECISION NOT NULL DEFAULT 0,
    "PricePerUnit" NUMERIC(18, 2) NOT NULL DEFAULT 0.00,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabel StockTransactions (Riwayat Barang Masuk & Terjual)
CREATE TABLE "StockTransactions" (
    "Id" SERIAL PRIMARY KEY,
    "TukangSayurId" INT NOT NULL REFERENCES "TukangSayurProfiles"("Id") ON DELETE CASCADE,
    "ProductId" INT NOT NULL REFERENCES "Products"("Id") ON DELETE RESTRICT,
    "Type" INT NOT NULL, -- 0: RestockIn (Barang Masuk), 1: SaleOut (Terjual)
    "Quantity" DOUBLE PRECISION NOT NULL,
    "UnitPrice" NUMERIC(18, 2) NOT NULL,
    "TotalAmount" NUMERIC(18, 2) NOT NULL,
    "Notes" TEXT NOT NULL DEFAULT '',
    "TransactionDate" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabel Orders (Pesanan Pelanggan)
CREATE TABLE "Orders" (
    "Id" SERIAL PRIMARY KEY,
    "CustomerId" INT NOT NULL REFERENCES "Users"("Id") ON DELETE RESTRICT,
    "TukangSayurId" INT NOT NULL REFERENCES "TukangSayurProfiles"("Id") ON DELETE RESTRICT,
    "TotalAmount" NUMERIC(18, 2) NOT NULL,
    "Status" INT NOT NULL DEFAULT 1, -- 0: Pending, 1: Completed, 2: Cancelled
    "MeetLatitude" DOUBLE PRECISION NOT NULL,
    "MeetLongitude" DOUBLE PRECISION NOT NULL,
    "MeetAddress" TEXT NOT NULL DEFAULT '',
    "OrderDate" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabel OrderItems (Detail Barang Pesanan)
CREATE TABLE "OrderItems" (
    "Id" SERIAL PRIMARY KEY,
    "OrderId" INT NOT NULL REFERENCES "Orders"("Id") ON DELETE CASCADE,
    "ProductId" INT NOT NULL REFERENCES "Products"("Id") ON DELETE RESTRICT,
    "Quantity" DOUBLE PRECISION NOT NULL,
    "UnitPrice" NUMERIC(18, 2) NOT NULL,
    "SubTotal" NUMERIC(18, 2) NOT NULL
);

-- ============================================================================
-- 3. INSERT SAMPLE SEED DATA
-- Password Salt yang digunakan di API: TS_SALT_2026 (SHA256 Base64)
-- ============================================================================

-- Data Users (Role: 0=Admin, 1=TukangSayur, 2=Pelanggan)
INSERT INTO "Users" ("Id", "FullName", "Email", "Phone", "PasswordHash", "Role", "Address", "Latitude", "Longitude", "CreatedAt") VALUES
(1, 'Administrator Sayur', 'admin@tukangsayur.com', '081234567890', '0tC3Cyl5gYI1tV9xL0N60S2Wl21iOsq3y064+wB0n6U=', 0, 'Kantor Pusat Tukang Sayur Online, Jakarta', -6.2088, 106.8456, CURRENT_TIMESTAMP),
(2, 'Mang Udin Sutarman', 'mang.udin@gmail.com', '081987654321', 'vN1bA6e9L++3N1j4gQ31oO4Gq1gLq+1wH/h/eFvW1z4=', 1, 'Jl. Kelapa Gading No. 12, Jakarta Utara', -6.1550, 106.9020, CURRENT_TIMESTAMP),
(3, 'Bang Budi Santoso', 'bang.budi@gmail.com', '081777888999', 'gQ1hE4vL+9N1j4gQ31oO4Gq1gLq+1wH/h/eFvW1z4=', 1, 'Jl. Tebet Raya No. 45, Jakarta Selatan', -6.2250, 106.8550, CURRENT_TIMESTAMP),
(4, 'Ibu Siti Aminah', 'pelanggan@gmail.com', '085611223344', 'aN2cE6vM+9N1j4gQ31oO4Gq1gLq+1wH/h/eFvW1z4=', 2, 'Jl. Sunter Garden No. 8, Jakarta Utara', -6.1480, 106.8720, CURRENT_TIMESTAMP);

SELECT setval('"Users_Id_seq"', (SELECT MAX("Id") FROM "Users"));

-- Data Profile Tukang Sayur
INSERT INTO "TukangSayurProfiles" ("Id", "UserId", "ShopName", "Balance", "IsOnline", "Latitude", "Longitude", "CurrentLocationName") VALUES
(1, 2, 'Sayur Segar Mang Udin', 350000.00, TRUE, -6.1550, 106.9020, 'Kelapa Gading Permai'),
(2, 3, 'Lapak Sayur Bang Budi', 520000.00, TRUE, -6.2250, 106.8550, 'Tebet Eco Park');

SELECT setval('"TukangSayurProfiles_Id_seq"', (SELECT MAX("Id") FROM "TukangSayurProfiles"));

-- Data Master Products
INSERT INTO "Products" ("Id", "Name", "Category", "Unit", "Description", "ImageUrl", "DefaultPrice", "CreatedAt") VALUES
(1, 'Bayam Hijau Segar', 'Sayuran Hijau', 'ikat', 'Bayam petik baru segar kaya zat besi', 'https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=400', 3500.00, CURRENT_TIMESTAMP),
(2, 'Kangkung Darat', 'Sayuran Hijau', 'ikat', 'Kangkung segar daun muda', 'https://images.unsplash.com/photo-1540420773420-3366772f4999?w=400', 3000.00, CURRENT_TIMESTAMP),
(3, 'Wortel Manis Dieng', 'Sayuran Umbi', 'kg', 'Wortel Dieng segar manis renyah', 'https://images.unsplash.com/photo-1598170845058-12ef4a457939?w=400', 12000.00, CURRENT_TIMESTAMP),
(4, 'Tomat Buah Merah', 'Sayuran Buah', 'kg', 'Tomat merah segar cocok untuk masakan & jus', 'https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=400', 14000.00, CURRENT_TIMESTAMP),
(5, 'Cabai Rawit Merah', 'Bumbu & Rempah', 'kg', 'Cabai rawit super pedas segar', 'https://images.unsplash.com/photo-1588252303782-cb80119abd6d?w=400', 45000.00, CURRENT_TIMESTAMP),
(6, 'Bawang Merah Brebes', 'Bumbu & Rempah', 'kg', 'Bawang merah olahan Brebes harum', 'https://images.unsplash.com/photo-1618512496248-a07fe83aa8cf?w=400', 38000.00, CURRENT_TIMESTAMP),
(7, 'Kentang Dieng Super', 'Sayuran Umbi', 'kg', 'Kentang Dieng besar mulus', 'https://images.unsplash.com/photo-1518977676601-b53f82aba655?w=400', 18000.00, CURRENT_TIMESTAMP),
(8, 'Tempe Organik Super', 'Lauk Pauk', 'papan', 'Tempe kedelai padat segar alami', 'https://images.unsplash.com/photo-1628102491629-778571d893a3?w=400', 6000.00, CURRENT_TIMESTAMP),
(9, 'Tahu Putih Halus', 'Lauk Pauk', 'bungkus', 'Tahu putih segar lembut 10 pcs', 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400', 7000.00, CURRENT_TIMESTAMP),
(10, 'Daging Ayam Broiler Segar', 'Daging & Ikan', 'kg', 'Daging ayam utuh segar potong', 'https://images.unsplash.com/photo-1604503468506-a8da13d82791?w=400', 36000.00, CURRENT_TIMESTAMP);

SELECT setval('"Products_Id_seq"', (SELECT MAX("Id") FROM "Products"));

-- Data Stok Mang Udin (TukangSayurId = 1)
INSERT INTO "TukangSayurStocks" ("TukangSayurId", "ProductId", "StockQuantity", "PricePerUnit", "UpdatedAt") VALUES
(1, 1, 20, 4000.00, CURRENT_TIMESTAMP),
(1, 2, 15, 3500.00, CURRENT_TIMESTAMP),
(1, 3, 10, 13000.00, CURRENT_TIMESTAMP),
(1, 4, 8, 15000.00, CURRENT_TIMESTAMP),
(1, 5, 0, 48000.00, CURRENT_TIMESTAMP), -- Stok kosong
(1, 8, 12, 6500.00, CURRENT_TIMESTAMP);

-- Data Stok Bang Budi (TukangSayurId = 2)
INSERT INTO "TukangSayurStocks" ("TukangSayurId", "ProductId", "StockQuantity", "PricePerUnit", "UpdatedAt") VALUES
(2, 1, 10, 4000.00, CURRENT_TIMESTAMP),
(2, 4, 15, 14500.00, CURRENT_TIMESTAMP),
(2, 5, 5, 47000.00, CURRENT_TIMESTAMP),
(2, 6, 12, 40000.00, CURRENT_TIMESTAMP),
(2, 7, 0, 19000.00, CURRENT_TIMESTAMP), -- Stok kosong
(2, 10, 6, 38000.00, CURRENT_TIMESTAMP);

-- Data Transaksi Awal Mang Udin
INSERT INTO "StockTransactions" ("TukangSayurId", "ProductId", "Type", "Quantity", "UnitPrice", "TotalAmount", "Notes", "TransactionDate") VALUES
(1, 1, 0, 30, 3000.00, 90000.00, 'Pembelian kulakan di Pasar Induk Kramat Jati', CURRENT_TIMESTAMP - INTERVAL '2 days'),
(1, 1, 1, 10, 4000.00, 40000.00, 'Penjualan ke Ibu Siti (Pelanggan)', CURRENT_TIMESTAMP - INTERVAL '1 day');

-- Done
