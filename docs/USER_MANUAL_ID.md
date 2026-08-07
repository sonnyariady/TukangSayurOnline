# 🥬 Dokumentasi & Panduan Pengguna: Tukang Sayur Online

---

## 📌 Ringkasan Eksekutif (Executive Summary)

**Tukang Sayur Online** adalah platform ekosistem perdagangan sayur hiper-lokal real-time yang menjembatani pedagang sayur keliling/lapak (**Tukang Sayur**), pengelola catalog pusat (**Admin**), dan pembeli rumah tangga (**Pelanggan**). 

Sistem ini memodernisasi cara kerja pedagang sayur tradisional dengan menyediakan fitur transparansi stok berbasis lokasi, pencatatan transaksi restock & penjualan kasir mandiri, status keaktifan toko (*Online/Offline toggle*), serta analitik barang paling laku dan pemantauan stok habis secara *real-time*.

---

## 🎯 Peran Pengguna (User Roles Matrix)

| Fitur / Modul | Admin | Tukang Sayur (Vendor) | Pelanggan (Customer) |
| :--- | :---: | :---: | :---: |
| **Pendaftaran & Otentikasi** | 🔒 *Pre-configured* | ✅ Registrasi Mandiri | ✅ Registrasi Mandiri |
| **Kelola Katalog Master Produk** | ✅ Buat, Edit, Hapus | ❌ *(Hanya melihat)* | ❌ *(Hanya melihat)* |
| **Input Restock / Stok Masuk** | ❌ | ✅ Pilih dari Katalog Master | ❌ |
| **Catat Penjualan Langsung (Offline)** | ❌ | ✅ Potong Stok + Tambah Saldo | ❌ |
| **Status Toko (Online / Offline Toggle)**| ❌ | ✅ Sakelar Keaktifan Toko | ❌ |
| **Cari Sayur Terdekat (Geo-Location)** | ❌ | ❌ | ✅ Berdasarkan jarak & stok |
| **Pembelian Online Produk** | ❌ | ❌ | ✅ Pilih jumlah & alamat |
| **Pantau Barang Kosong & Analitik** | ✅ Laporan Terpusat | ✅ Indikator Stok Habis | ❌ Auto-hide stok kosong |

---

## 📖 Panduan Pengguna Langkah-demi-Langkah (Step-by-Step User Manual)

### Modul 1: Tukang Sayur (Vendor / Merchant)

#### 1. Pendaftaran Akun Tukang Sayur
1. Akses halaman pendaftaran (`/register`).
2. Pilih peran **Tukang Sayur**.
3. Lengkapi formulir pendaftaran:
   - **Nama Lengkap**: Nama pemilik toko (contoh: *Bang Jhon*).
   - **Email & No. Telepon/WA**: Untuk verifikasi dan kontak.
   - **Password**: Kata sandi akun.
   - **Nama Toko / Lapak Sayur**: Nama usaha (contoh: *Toko Sayur Bang Jhon*).
   - **Alamat Domisili / Lokasi Jualan**: Alamat operasional (contoh: *Jalan H. Nawin*).
4. Klik tombol **Daftar Akun**.

![Form Registrasi](file:///c:/Latihan/TukangSayurOnline/docs/images/01_register_page.png)

#### 2. Menambah Stok Masuk (Restock Barang)
1. Setelah login, Anda akan diarahkan ke **Dashboard Tukang Sayur** (`/tukangsayur`).
2. Klik tombol hijau **`INPUT STOK MASUK (RESTOCK)`**.
3. Modal dialog akan muncul:
   - **Pilih Barang**: Pilih produk dari Katalog Master (contoh: *Tahu Putih Halus*, *Cabai Rawit Merah*, *Daging Ayam Broiler Segar*).
   - **Jumlah Qty Dibeli**: Masukkan jumlah unit (contoh: *33 bungkus*).
   - **Harga Beli Per Satuan**: Masukkan harga modal/restock.
   - **Catatan (Lokasi Pasar/Kulakan)**: Catat lokasi pasar tempat kulakan (contoh: *Pasar Bersih*).
4. Klik **Simpan Barang Masuk**. Total pengeluaran restock dan jumlah stok lokal akan terupdate secara otomatis.

![Restock Modal](file:///c:/Latihan/TukangSayurOnline/docs/images/03_restock_modal.png)
![Dashboard Tukang Sayur](file:///c:/Latihan/TukangSayurOnline/docs/images/02_vendor_dashboard.png)

#### 3. Pencatatan Penjualan Langsung (Kasir Offline)
1. Pada Dashboard Tukang Sayur, klik tombol kuning **`CATAT PENJUALLAN LANGSUNG`**.
2. Modal dialog akan terbuka:
   - **Pilih Barang Terjual**: Pilih item yang terjual.
   - **Jumlah Qty Terjual**: Masukkan kuantitas yang dibeli pelanggan secara langsung.
   - **Harga Jual Per Unit**: Harga jual retail.
   - **Catatan Pembeli / Lokasi Transaksi**: Alamat atau pembeli (contoh: *Di Jalan Camar V*).
3. Klik **Proses Penjualan (+Saldo)**.
4. Stok barang akan terpotong secara instan, dan saldo dompet aplikasi Anda akan bertambah secara real-time.

#### 4. Pengaturan Status Operasional (Toggle Online / Offline)
- Pada bagian header Dashboard Tukang Sayur, terdapat sakelar **`Status Online`**.
- Geser sakelar ke **ONLINE (SIAP JUALAN)** saat siap menerima pesanan dari pelanggan.
- Geser sakelar ke **OFFLINE** saat sedang beristirahat, tidur, atau belum siap berjualan. Toko dan barang Anda secara otomatis akan disembunyikan dari aplikasi pelanggan.

---

### Modul 2: Administrator (Admin Portal)

#### 1. Pengelolaan Master Data Barang
1. Akses **Dashboard Admin** (`/admin`).
2. Pada tab **`MASTER DATA BARANG`**, Admin dapat menambah dan memperbarui katalog global:
   - Gambar produk, Nama barang, Kategori (misal: *Bumbu & Rempah*, *Daging & Ikan*, *Sayuran Buah*), Satuan (*kg*, *bungkus*, *papan*, *ikat*), Harga default, dan Deskripsi.

![Dashboard Admin Master Data](file:///c:/Latihan/TukangSayurOnline/docs/images/04_admin_dashboard.png)

#### 2. Analitik Produk Terlaris & Pemantauan Barang Kosong
1. Pilih tab **`PRODUK TERLARIS (ANALISTIK)`** untuk melihat peringkat komoditas paling banyak terjual.
2. Pilih tab **`BARANG KOSONG TUKANG SAYUR`** untuk memantau toko/lapak pedagang mana saja yang stoknya telah habis (Stok = 0).

---

### Modul 3: Pelanggan (Customer / Buyer)

#### 1. Registrasi & Discovery Sayur Terdekat
1. Akses halaman `/register` untuk pendaftaran akun Pelanggan.
2. Buka **Dashboard Pelanggan** (`/pelanggan`).
3. Sistem secara otomatis menampilkan daftar **Tukang Sayur Terdekat (Ready Stock)** yang berstatus **ONLINE**.

![Dashboard Pelanggan](file:///c:/Latihan/TukangSayurOnline/docs/images/05_customer_dashboard.png)

#### 2. Melakukan Transaksi Pembelian
1. Klik tombol **`TEMUI & BELI LANGSUNG`** pada produk yang diinginkan.
2. Masukkan jumlah kuantitas dan **Alamat Delivery / Titik Temu**.
3. Klik **Konfirmasi Transaksi**. Notifikasi sukses akan muncul dan stok pedagang akan berkurang secara real-time.

---

## 🎨 Panduan Cetak / Export Dokumentasi ke PDF

Seluruh berkas dokumentasi ini disiapkan agar dapat dicetak menjadi buku panduan & portofolio profesional:
1. Buka file **[PORTFOLIO_PRESENTATION.html](file:///c:/Latihan/TukangSayurOnline/docs/PORTFOLIO_PRESENTATION.html)**.
2. Klik tombol **`🖨️ Cetak / Simpan PDF Portofolio`** atau tekan `Ctrl + P`.
3. Pilih opsi **Simpan sebagai PDF**.
