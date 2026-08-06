# 🥬 Tukang Sayur Online (Vegetable Seller Online System)

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14%2B-4169E1?style=for-the-badge&logo=postgresql)](https://www.postgresql.org/)
[![Blazor Web](https://img.shields.io/badge/Blazor-Web_UI-512BD4?style=for-the-badge&logo=blazor)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![MAUI Hybrid](https://img.shields.io/badge/MAUI-Hybrid_Blazor-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/apps/maui)
[![MudBlazor](https://img.shields.io/badge/MudBlazor-7.4.0-7E57C2?style=for-the-badge&logo=blazor)](https://mudblazor.com/)
[![JWT Auth](https://img.shields.io/badge/Authentication-JWT_Bearer-000000?style=for-the-badge&logo=jsonwebtokens)](https://jwt.io/)

Aplikasi **Tukang Sayur Online** adalah platform ekosistem perdagangan sayur berbasis lokasi dan real-time stok yang mengintegrasikan tiga peran utama: **Admin**, **Tukang Sayur (Seller/Vendor)**, dan **Pelanggan (Customer)**.

Sistem dirancang dengan arsitektur **Multi-Project Enterprise Code Sharing**:
- **`TukangSayurOnline.Shared`**: Razor Class Library (RCL) berbasis **MudBlazor 7.4.0** tempat beradanya Halaman UI, Component Layout, dan Service API yang dapat digunakan bersama oleh aplikasi Web dan Mobile.
- **`TukangSayurOnline.Web`**: Aplikasi Web Browser (Blazor Web App .NET 9).
- **`TukangSayurOnline.Mobile`**: Aplikasi Mobile & Desktop (.NET 9 MAUI Blazor Hybrid).
- **`TukangSayurOnline.Api`**: Backend RESTful Web API (.NET 9) dengan database **PostgreSQL**.

---

## 📋 Daftar Isi
1. [Arsitektur Solution & Code Sharing](#-arsitektur-solution--code-sharing)
2. [Prasyarat Sistem (Prerequisites)](#-prasyarat-sistem-prerequisites)
3. [Panduan Langkah-demi-Langkah Menjalankan Aplikasi](#-panduan-langkah-demi-langkah-menjalankan-aplikasi)
4. [Fitur Utama & Panduan Pengujian Role](#-fitur-utama--panduan-pengujian-role)
5. [Dokumentasi RESTful API (Swagger)](#-dokumentasi-restful-api-swagger)
6. [Petunjuk Commit & Push ke GitHub](#-petunjuk-commit--push-ke-github)

---

## 🏗️ Arsitektur Solution & Code Sharing

```mermaid
graph TD
    subgraph Shared UI Layer [TukangSayurOnline.Shared - Razor Class Library]
        Services[ApiService & AppStateService]
        Layouts[MainLayout & MudTheme]
        Pages[Login, Register, Admin, TukangSayur & Pelanggan Dashboards]
    end

    subgraph Platform UI Clients
        WebUI[TukangSayurOnline.Web - Blazor Web App] --> Shared UI Layer
        MobileUI[TukangSayurOnline.Mobile - MAUI Blazor Hybrid] --> Shared UI Layer
    end

    subgraph Backend API & Database
        API[TukangSayurOnline.Api - ASP.NET Core 9.0 API]
        DB[(PostgreSQL DbTukangSayurOnline)]
    end

    WebUI -->|HTTP REST JSON| API
    MobileUI -->|HTTP REST JSON| API
    API --> DB
```

### Struktur Project dalam Solution (`TukangSayurOnline.sln`):
```text
c:\Latihan\TukangSayurOnline\
├── init_db.sql                         <-- Script Manual Execute SQL (Tabel + Sample Data)
├── README.md                           <-- Dokumentasi Panduan Jalankan Aplikasi
├── TukangSayurOnline.sln              <-- Solution File (.NET 9 Solution)
├── src/
│   ├── TukangSayurOnline.Shared/       <-- Shared UI Library (MudBlazor Pages, Layout & Services)
│   ├── TukangSayurOnline.Web/          <-- Web App Host (Blazor Web App .NET 9)
│   ├── TukangSayurOnline.Mobile/       <-- Mobile Hybrid App Host (MAUI Hybrid .NET 9)
│   └── TukangSayurOnline.Api/          <-- Backend Web API (.NET 9 + EF Core PostgreSQL)
```

---

## 🛠️ Prasyarat Sistem (Prerequisites)

Sebelum menjalankan aplikasi, pastikan komputer Anda telah terinstall perangkat lunak berikut:
1. **.NET 9.0 SDK** (`dotnet --version` >= 9.0.100)
2. **PostgreSQL Database 14+** (Berjalan di `localhost:5432`)
3. **pgAdmin 4** / **DBeaver** / `psql` command line (Opsional untuk mengeksekusi SQL manual)

---

## 🚀 Panduan Langkah-demi-Langkah Menjalankan Aplikasi

Ikuti urutan langkah di bawah ini untuk memulai seluruh ekosistem aplikasi:

### 📍 Langkah 1: Persiapan Database PostgreSQL

Pastikan PostgreSQL service Anda telah aktif di `localhost:5432`.

Kredensial Database:
- **Database Name**: `DbTukangSayurOnline`
- **Username**: `appuser`
- **Password**: `AppPass123`
- **Connection String**: `Host=localhost;Port=5432;Database=DbTukangSayurOnline;Username=appuser;Password=AppPass123;Pooling=true;Include Error Detail=true`

#### **Metode A: Eksekusi Script SQL Manual (Sangat Direkomendasikan)**
Script [init_db.sql](file:///c:/Latihan/TukangSayurOnline/init_db.sql) sudah menyertakan pembuatan tabel dan data awal sampel.

1. Buka PostgreSQL Command Line / Terminal:
   ```bash
   psql -U appuser -d postgres
   ```
2. Buat Database:
   ```sql
   CREATE DATABASE "DbTukangSayurOnline";
   ```
3. Eksekusi file SQL [init_db.sql](file:///c:/Latihan/TukangSayurOnline/init_db.sql):
   ```bash
   psql -U appuser -d DbTukangSayurOnline -f c:\Latihan\TukangSayurOnline\init_db.sql
   ```
   *(Atau jalankan isi file `init_db.sql` di Query Tool pada **pgAdmin** / **DBeaver**)*.

#### **Metode B: Auto Migration EF Core**
Jika Anda melompati eksekusi file SQL, saat Backend API pertama kali dijalankan di Langkah 2, EF Core akan secara otomatis membuatkan database `DbTukangSayurOnline` beserta seluruh tabel & data bawaannya.

---

### 📍 Langkah 2: Jalankan Backend Web API (.NET 9.0)

Buka Terminal / PowerShell di folder root project:

```powershell
cd c:\Latihan\TukangSayurOnline
dotnet run --project src/TukangSayurOnline.Api/TukangSayurOnline.Api.csproj
```

Output terminal akan menunjukkan API siap melayani request di:
`http://localhost:5000`

---

### 📍 Langkah 3: Uji Dokumentasi Swagger API

Buka Web Browser pilihan Anda dan akses alamat:
👉 **[http://localhost:5000/swagger](http://localhost:5000/swagger)**

Anda dapat menguji endpoint JWT Login, Register, Master Produk, Restock Stok, Sale Out, dan Pencarian Tukang Sayur Terdekat secara langsung.

---

### 📍 Langkah 4: Jalankan Frontend Web App (Web Browser)

Buka jendela Terminal / PowerShell baru untuk menjalankan aplikasi Web UI:

```powershell
cd c:\Latihan\TukangSayurOnline
dotnet run --project src/TukangSayurOnline.Web/TukangSayurOnline.Web.csproj
```

Setelah server aktif, buka browser di alamat yang tampil pada terminal (misal: `http://localhost:5100` atau `http://localhost:5200`).

---

### 📍 Langkah 5: Jalankan Aplikasi Mobile / Hybrid (MAUI Blazor)

Buka jendela Terminal / PowerShell baru untuk menjalankan aplikasi Mobile / Windows Desktop:

```powershell
cd c:\Latihan\TukangSayurOnline
dotnet run --project src/TukangSayurOnline.Mobile/TukangSayurOnline.Mobile.csproj -f net9.0-windows10.0.19041.0
```

---

## 🔑 Fitur Utama & Panduan Pengujian Role

Gunakan akun pengujian bawaan di bawah ini (atau gunakan tombol **Akses Cepat Akun Demo** di layar Login):

| Peran (Role) | Username / Email | Password | Panduan Alur Pengujian & Fitur Utama |
| :--- | :--- | :--- | :--- |
| **👑 Admin** | `admin@tukangsayur.com` | `Admin123!` | 1. Login -> Diarahkan ke **Dashboard Admin**.<br>2. **Tab Master Barang**: Tambah, Edit, atau Hapus katalog barang.<br>3. **Tab Produk Terlaris**: Lihat laporan barang mana yang paling laku terjual.<br>4. **Tab Barang Kosong**: Cek stok 0 di lapak para tukang sayur. |
| **🚴 Tukang Sayur 1** | `mang.udin@gmail.com` | `Udin123!` | 1. Login -> Diarahkan ke **Dashboard Tukang Sayur**.<br>2. Toko: **Sayur Segar Mang Udin**, Saldo Aktif: Rp 350.000.<br>3. Klik **Input Stok Masuk (Restock)** untuk menambah barang kulakan.<br>4. Klik **Catat Penjualan Direct** -> Stok berkurang & Saldo bertambah. |
| **🚴 Tukang Sayur 2** | `bang.budi@gmail.com` | `Budi123!` | Toko: **Lapak Sayur Bang Budi**, Saldo: Rp 520.000, Kelola Stok & Harga Jual. |
| **🛒 Pelanggan** | `pelanggan@gmail.com` | `Pelanggan123!` | 1. Login -> Diarahkan ke **Belanja Pelanggan**.<br>2. Cari produk (misal: "Bayam" / "Wortel").<br>3. Sistem menampilkan Tukang Sayur terdekat + stok + jarak km (Haversine Formula).<br>4. Klik **Temui & Beli Langsung** -> Konfirmasi order -> Stok tukang sayur terpotong & saldo tukang sayur bertambah. |

---

## 📡 Dokumentasi RESTful API (Swagger Endpoints)

- **Auth**:
  - `POST /api/auth/register`: Pendaftaran Pelanggan & Tukang Sayur.
  - `POST /api/auth/login`: Authentication & penerbitan token JWT.
- **Admin**:
  - `GET /api/admin/products`: Ambil master produk (dengan filter category & search).
  - `POST /api/admin/products`: Tambah master barang baru.
  - `PUT /api/admin/products/{id}`: Edit master barang.
  - `DELETE /api/admin/products/{id}`: Hapus master barang.
  - `GET /api/admin/reports/popular-products`: Laporan barang terlaris.
  - `GET /api/admin/reports/empty-stocks`: Laporan barang kosong per tukang sayur.
- **Tukang Sayur**:
  - `GET /api/tukangsayur/{id}/stocks`: Ambil daftar stok milik tukang sayur.
  - `POST /api/tukangsayur/{id}/restock-in`: Transaksi stok masuk (kulakan).
  - `POST /api/tukangsayur/{id}/sale-out`: Transaksi penjualan langsung (+Saldo).
  - `GET /api/tukangsayur/{id}/income-summary`: Ringkasan saldo & omset.
  - `POST /api/tukangsayur/{id}/toggle-online`: Ubah status online & koordinat GPS.
- **Pelanggan**:
  - `GET /api/pelanggan/search-nearby`: Pencarian tukang sayur terdekat (Haversine formula).
  - `POST /api/pelanggan/orders`: Buat transaksi pembelian & penentuan lokasi pertemuan.
  - `GET /api/pelanggan/orders/my-orders`: Histori belanjaan pelanggan.

---

## 📤 Petunjuk Commit & Push ke GitHub

Jalankan perintah berikut di PowerShell untuk mengunggah seluruh codebase ke GitHub:

```powershell
# 1. Inisialisasi Repository Git (jika belum dilakukan)
git init

# 2. Add seluruh file project (diatur oleh .gitignore)
git add .

# 3. Commit perubahan
git commit -m "feat: Add Web UI app, Shared RCL library, PostgreSQL setup, and comprehensive documentation"

# 4. Hubungkan ke Remote GitHub (Ganti URL sesuai repository Anda)
git remote add origin https://github.com/USERNAME/TukangSayurOnline.git

# 5. Push ke branch main
git branch -M main
git push -u origin main
```
