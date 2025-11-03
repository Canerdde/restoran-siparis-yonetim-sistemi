using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RestoranSiparisYonetimi
{
    // File işlemleri için sınıf
    public class FileManager
    {
        private string _orderHistoryPath = "order_history.txt";
        private string _dailyReportsPath = "daily_reports.txt";

        // Sipariş geçmişini dosyaya kaydet
        public void SaveOrder(Order order)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(_orderHistoryPath))
                {
                    writer.WriteLine($"=== Sipariş #{order.OrderId} ===");
                    writer.WriteLine($"Masa: {order.Table.TableNumber}");
                    writer.WriteLine($"Tarih: {order.OrderTime:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine("Ürünler:");
                    
                    foreach (var item in order.Items)
                    {
                        writer.WriteLine($"  - {item.MenuItem.Name} x {item.Quantity} = {item.Subtotal:C}");
                    }
                    
                    writer.WriteLine($"Toplam: {order.TotalAmount:C}");
                    writer.WriteLine(new string('-', 50));
                    writer.WriteLine();
                }

                Console.WriteLine($"[DOSYA] Sipariş #{order.OrderId} dosyaya kaydedildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA] Dosyaya yazma hatası: {ex.Message}");
            }
        }

        // Gün sonu raporunu dosyaya kaydet
        public void SaveDailyReport(string report, DateTime date)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(_dailyReportsPath))
                {
                    writer.WriteLine(report);
                    writer.WriteLine();
                }

                Console.WriteLine($"[DOSYA] {date:yyyy-MM-dd} gün sonu raporu kaydedildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA] Rapor kaydetme hatası: {ex.Message}");
            }
        }

        // Dosyadan sipariş geçmişini oku
        public void ReadOrderHistory()
        {
            try
            {
                if (!File.Exists(_orderHistoryPath))
                {
                    Console.WriteLine("[BİLGİ] Henüz kayıtlı sipariş yok.");
                    return;
                }

                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("SİPARİŞ GEÇMİŞİ");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine(File.ReadAllText(_orderHistoryPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA] Dosya okuma hatası: {ex.Message}");
            }
        }

        // Dosyadan gün sonu raporlarını oku
        public void ReadDailyReports()
        {
            try
            {
                if (!File.Exists(_dailyReportsPath))
                {
                    Console.WriteLine("[BİLGİ] Henüz rapor yok.");
                    return;
                }

                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("GÜN SONU RAPORLARI");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine(File.ReadAllText(_dailyReportsPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA] Rapor okuma hatası: {ex.Message}");
            }
        }
    }
}

