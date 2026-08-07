# 🥬 Documentation & User Manual: Tukang Sayur Online

---

## 📌 Executive Summary

**Tukang Sayur Online** is a real-time hyper-local vegetable commerce platform designed to bridge traditional mobile vegetable vendors (**Tukang Sayur**), platform administrators (**Admin**), and residential buyers (**Customer**).

The system modernizes traditional vegetable selling workflows by providing location-aware stock visibility, self-service restock & point-of-sale (POS) transaction recording, shop availability toggling (*Online/Offline toggle*), best-selling product analytics, and real-time out-of-stock monitoring.

---

## 🎯 User Roles Matrix

| Feature / Module | Admin | Tukang Sayur (Vendor) | Customer (Pelanggan) |
| :--- | :---: | :---: | :---: |
| **Registration & Auth** | 🔒 *Pre-configured* | ✅ Self-Registration | ✅ Self-Registration |
| **Global Master Catalog Management** | ✅ Create, Edit, Delete | ❌ *(Read-only)* | ❌ *(Read-only)* |
| **Inventory Restock Input** | ❌ | ✅ Select from Master Catalog | ❌ |
| **Direct Offline Sales Recording** | ❌ | ✅ Deduct Stock + Add Balance | ❌ |
| **Shop Status (Online / Offline Toggle)**| ❌ | ✅ Availability Switch | ❌ |
| **Nearby Vendor Search (Geo-Location)** | ❌ | ❌ | ✅ By distance & stock |
| **Online Product Purchasing** | ❌ | ❌ | ✅ Select qty & delivery address |
| **Out-of-Stock & Analytics Monitor** | ✅ Centralized Report | ✅ Out of Stock Indicator | ❌ Auto-hides zero stock |

---

## 📖 Step-by-Step User Manual

### Module 1: Vendor (Tukang Sayur / Merchant)

#### 1. Vendor Account Registration
1. Access the registration page (`/register`).
2. Select the **Tukang Sayur** role.
3. Complete registration: Full Name (*Bang Jhon*), Email, WhatsApp, Password, Store Name (*Toko Sayur Bang Jhon*), Domicile Address (*Jalan H. Nawin*).
4. Click **Register Account**.

![Registration Form](file:///c:/Latihan/TukangSayurOnline/docs/images/01_register_page.png)

#### 2. Inventory Restock Input (Stock In)
1. Upon logging in, navigate to the **Vendor Dashboard** (`/tukangsayur`).
2. Click the green button **`INPUT STOK MASUK (RESTOCK)`**.
3. Fill in product details, purchased quantity, unit cost price, and wholesale market notes.
4. Click **Save Stock In**. Local inventory and restock totals will update automatically.

![Restock Modal](file:///c:/Latihan/TukangSayurOnline/docs/images/03_restock_modal.png)
![Vendor Dashboard](file:///c:/Latihan/TukangSayurOnline/docs/images/02_vendor_dashboard.png)

#### 3. Recording Direct Offline Sales (POS Cashier)
1. On the Vendor Dashboard, click the yellow button **`CATAT PENJUALLAN LANGSUNG`**.
2. Select the sold product, quantity, selling price, and customer location note.
3. Click **Process Sale (+Balance)**.
4. Inventory deducts instantly, and application wallet balance increases in real-time.

---

### Module 2: Administrator (Admin Portal)

#### 1. Master Product Catalog Management
1. Access the **Admin Dashboard** (`/admin`).
2. On the **`MASTER DATA BARANG`** tab, create and update global catalog items (images, names, categories, units, default prices, descriptions).

![Admin Dashboard](file:///c:/Latihan/TukangSayurOnline/docs/images/04_admin_dashboard.png)

#### 2. Analytics & Out-of-Stock Monitoring
1. Select **`PRODUK TERLARIS (ANALISTIK)`** to view top-selling products.
2. Select **`BARANG KOSONG TUKANG SAYUR`** to track vendors with zero stock.

---

### Module 3: Customer (Pelanggan / Buyer)

#### 1. Customer Discovery & Purchasing
1. Access `/register` to create a Customer account.
2. Open the **Customer Dashboard** (`/pelanggan`).
3. View nearby **ONLINE** vendors sorted by distance (e.g. 3.41 km) with available inventory.

![Customer Dashboard](file:///c:/Latihan/TukangSayurOnline/docs/images/05_customer_dashboard.png)

4. Click **`TEMUI & BELI LANGSUNG`**, specify quantity and delivery address, and confirm transaction.

---

## 🎨 Exporting Documentation to PDF

All documentation files can be printed into a professional PDF showcase:
1. Open **[PORTFOLIO_PRESENTATION.html](file:///c:/Latihan/TukangSayurOnline/docs/PORTFOLIO_PRESENTATION.html)**.
2. Click **`🖨️ Cetak / Simpan PDF Portofolio`** or press `Ctrl + P`.
3. Choose **Save as PDF**.
