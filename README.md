# 🍽️ Restoran Sipariş Yönetim Sistemi

Modern Windows Forms masaüstü uygulaması ile geliştirilmiş, nesne tabanlı programlama kavramlarını kapsamlı şekilde uygulayan profesyonel bir restoran yönetim sistemi.

## 📋 Proje Amacı

Interface, event ve collection kavramlarını bir arada kullanarak gerçek hayat senaryosuna uygun bir sipariş yönetim sistemi geliştirmek.

## 🎯 Temel Özellikler

- ✅ **Sipariş Yönetimi**: Masa bazlı sipariş oluşturma ve takibi
- ✅ **Detaylı Raporlama**: LINQ ile analiz ve görselleştirme
- ✅ **Event-Driven Mimari**: Otomatik bildirim ve kayıt sistemi
- ✅ **Kalıcı Veri Saklama**: Dosya tabanlı sipariş ve rapor arşivi
- ✅ **Modern UI**: Kullanıcı dostu Windows Forms arayüzü

## 🏗️ Mimari ve OOP Kavramları

### 1️⃣ Class Yapıları

```csharp
// Temel Sınıflar
Table       → Masa bilgileri (numara, kapasite, durum)
Menu        → Ürün base class (abstract)
├─ Food     → Yiyecekler (inheritance)
└─ Beverage → İçecekler (inheritance)
Item        → Sipariş kalemi
Order       → Sipariş (IOrder interface implementasyonu)
```

### 2️⃣ Interface Kullanımı

```csharp
public interface IOrder
{
    int OrderId { get; }
    Table Table { get; }
    List<Item> Items { get; }
    void AddItem(Item item);
    void CalculateTotal();
}
```
**Neden?** Sipariş davranışlarını standardize eder, farklı sipariş türleri eklenebilir.

### 3️⃣ Event-Driven Sistem

```csharp
// Event Tanımı
public event EventHandler<OrderEventArgs> NewOrderReceived;

// Event Tetikleme
manager.AddOrder(order) → Event fires → Dosyaya kaydet + İstatistik güncelle
```

**Çalışma Mantığı:**
```
[Sipariş Al] → [Event Tetikle] → [Tüm Subscriber'lar Çalışır]
                                  ├─ Dosyaya Kaydet
                                  ├─ İstatistik Güncelle
                                  └─ UI Güncelle
```

### 4️⃣ Collection Kullanımı

```csharp
List<Order> _allOrders;     // Tüm siparişler
List<Item> Items;           // Sipariş kalemleri
Menu[] menuItems;           // Ürün listesi
Table[] tables;             // Masa listesi
```

### 5️⃣ LINQ ile Veri Analizi

```csharp
// Gün sonu raporu için kullanılan LINQ metodları
var dailyOrders = _allOrders
    .Where(o => o.OrderTime.Date == date.Date)          // Filtreleme
    .GroupBy(o => o.Table.TableNumber)                   // Gruplama
    .Select(g => new { ... })                            // Projeksiyon
    .OrderByDescending(x => x.Revenue)                   // Sıralama
    .Sum(o => o.TotalAmount)                             // Toplam
    .Average(o => o.TotalAmount)                         // Ortalama
    .Max(o => o.TotalAmount)                             // En büyük
    .SelectMany(o => o.Items)                            // Düzleştirme
    .Take(5);                                            // İlk 5
```

**Analiz Çıktıları:**
- 📊 Toplam ciro ve sipariş sayısı
- 🏆 En çok sipariş alan masa
- 🕐 En yoğun saat dilimi
- 🥇 En çok satan ürünler (Top 5)
- 📈 Kategori bazlı satış yüzdeleri

### 6️⃣ File Operations

```csharp
// YAZMA
using (StreamWriter writer = File.AppendText("order_history.txt"))
{
    writer.WriteLine($"Sipariş #{order.OrderId}");
}

// OKUMA
string content = File.ReadAllText("order_history.txt");

// KONTROL
if (File.Exists(path)) { ... }
```

**Kaydedilen Dosyalar:**
- `order_history.txt` → Her sipariş detayı
- `daily_reports.txt` → Gün sonu raporları

## 🔄 Program Akışı

```mermaid
1. Program Başlat
   ↓
2. Manager & Collections Initialize
   ↓
3. Event Handler'ları Subscribe Et
   ↓
4. UI Göster (MainForm)
   ↓
5. Kullanıcı Sipariş Oluşturur
   ├─ Masa Seç
   ├─ Ürün Ekle (Collection'a)
   └─ Toplam Hesapla (LINQ Sum)
   ↓
6. Siparişi Onayla
   ↓
7. Event Tetiklenir (NewOrderReceived)
   ├─ FileManager.SaveOrder() → Dosyaya yaz
   └─ UpdateStats() → UI güncelle
   ↓
8. Gün Sonu Raporu İste
   ↓
9. LINQ Sorguları Çalışır
   ├─ Where, GroupBy, Select
   ├─ Sum, Average, Max/Min
   └─ OrderBy, Take
   ↓
10. ReportForm Açılır (Görsel Rapor)
    ↓
11. DailyReportGenerated Event → Rapora dosyaya kaydet
```

## 📁 Dosya Yapısı

```
📦 RestoranSiparisYonetimi/
├─📄 IOrder.cs                 # Interface tanımı
├─📄 Table.cs                  # Masa sınıfı
├─📄 Menu.cs                   # Ürün base class + Food, Beverage
├─📄 Item.cs                   # Sipariş kalemi
├─📄 Order.cs                  # Sipariş sınıfı (IOrder implement)
├─📄 RestaurantManager.cs      # Event & LINQ işlemleri
├─📄 FileManager.cs            # Dosya I/O işlemleri
├─📄 MainForm.cs               # Ana UI
├─📄 ReportForm.cs             # Rapor görüntüleme UI
├─📄 Program.cs                # Entry point
├─📄 README.md                 # Dokümantasyon
└─📄 RestoranSiparisYonetimi.csproj
```

## 🚀 Kurulum ve Çalıştırma

```bash
# Projeyi derle
dotnet build

# Uygulamayı çalıştır
dotnet run
```

**Gereksinimler:**
- .NET 8.0 SDK
- Windows OS (Windows Forms)

## 💡 Kullanım

### Sipariş Oluşturma
1. Masa seçin
2. Menüden ürün seçin ve adeti belirleyin
3. "Sepete Ekle" butonuna tıklayın
4. "Siparişi Tamamla" ile onaylayın

### Rapor Görüntüleme
- **Gün Sonu Raporu**: Detaylı analiz ve grafikler
- **Sipariş Geçmişi**: Tüm siparişlerin listesi

### Arama ve Filtreleme
- Kategori bazlı filtreleme (Ana Yemek, Çorba, İçecek)
- Ürün adı ile arama

## 📊 Ekran Görüntüleri

**Ana Ekran:**
- 📈 Gerçek zamanlı istatistikler
- 📋 Menü listesi ve arama
- 🛒 Sipariş sepeti

**Rapor Ekranı:**
- 📊 Detaylı grafikler
- 🥇 Top 5 ürünler
- 📈 Kategori analizi (progress bar ile)

## 🎓 Öğrenilen Kavramlar

### Nesne Tabanlı Programlama
- ✅ **Class & Object**: Temel yapı taşları
- ✅ **Inheritance**: Menu → Food, Beverage
- ✅ **Polymorphism**: DisplayInfo() override
- ✅ **Encapsulation**: Private fields, public properties
- ✅ **Abstraction**: Abstract Menu class

### İleri Seviye Konular
- ✅ **Interface**: IOrder implementasyonu
- ✅ **Event & Delegate**: Event-driven architecture
- ✅ **Collection**: List, Array kullanımı
- ✅ **LINQ**: 10+ farklı LINQ metodu
- ✅ **File I/O**: StreamWriter, File operations
- ✅ **Windows Forms**: Modern UI tasarımı

## 🔧 Teknik Detaylar

**Teknolojiler:**
- C# 10
- .NET 8.0
- Windows Forms
- LINQ
- System.IO

**Tasarım Desenleri:**
- Event-Driven Architecture
- Repository Pattern (RestaurantManager)
- Observer Pattern (Events)

## 📝 Notlar

- Tüm siparişler ve raporlar otomatik olarak dosyaya kaydedilir
- Uygulama kapatılıp açıldığında veriler kaybolmaz
- Event sistemi sayesinde loose coupling sağlanmıştır
- LINQ ile performanslı veri analizi yapılır