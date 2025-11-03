using System;
using System.Collections.Generic;
using System.Linq;

namespace RestoranSiparisYonetimi
{
    // Event argument sınıfı
    public class OrderEventArgs : EventArgs
    {
        public Order Order { get; set; }
        public DateTime EventTime { get; set; }

        public OrderEventArgs(Order order)
        {
            Order = order;
            EventTime = DateTime.Now;
        }
    }

    // Restoran yöneticisi sınıfı - event ve collection kullanımı
    public class RestaurantManager
    {
        // Event tanımı
        public event EventHandler<OrderEventArgs>? NewOrderReceived;
        public event EventHandler<string>? DailyReportGenerated;

        // Collection: Tüm siparişler
        private List<Order> _allOrders;

        public RestaurantManager()
        {
            _allOrders = new List<Order>();
        }

        // Yeni sipariş geldiğinde event tetiklenir
        public void AddOrder(Order order)
        {
            _allOrders.Add(order);
            
            // Event tetikle
            OnNewOrderReceived(new OrderEventArgs(order));
            
            Console.WriteLine($"\n[SİSTEM] Yeni sipariş alındı!");
            Console.WriteLine($"Sipariş No: {order.OrderId}");
            Console.WriteLine($"Tarih: {order.OrderTime:yyyy-MM-dd HH:mm:ss}");
        }

        // Event'i tetikleyen metod
        protected virtual void OnNewOrderReceived(OrderEventArgs e)
        {
            NewOrderReceived?.Invoke(this, e);
        }

        // LINQ ile gün sonu toplam satış raporu
        public string GenerateDailyReport(DateTime date)
        {
            var dailyOrders = _allOrders.Where(o => o.OrderTime.Date == date.Date).ToList();

            if (!dailyOrders.Any())
            {
                return $"\n{date:yyyy-MM-dd} tarihinde sipariş bulunamadı.";
            }

            var totalRevenue = dailyOrders.Sum(o => o.TotalAmount);
            var totalOrders = dailyOrders.Count;
            var averageOrder = dailyOrders.Average(o => o.TotalAmount);
            var maxOrder = dailyOrders.Max(o => o.TotalAmount);
            var minOrder = dailyOrders.Min(o => o.TotalAmount);

            // En çok sipariş verilen masa
            var topTable = dailyOrders
                .GroupBy(o => o.Table.TableNumber)
                .OrderByDescending(g => g.Count())
                .First();

            // Rapor oluştur
            string report = $"\n{new string('=', 60)}\n";
            report += $"GÜN SONU SATIŞ RAPORU\n";
            report += $"Tarih: {date:yyyy-MM-dd}\n";
            report += $"{new string('=', 60)}\n\n";
            report += $"Toplam Sipariş Sayısı: {totalOrders}\n";
            report += $"Toplam Ciro: {totalRevenue:C}\n";
            report += $"Ortalama Sipariş Tutarı: {averageOrder:C}\n";
            report += $"En Yüksek Sipariş: {maxOrder:C}\n";
            report += $"En Düşük Sipariş: {minOrder:C}\n";
            report += $"En Çok Sipariş Alan Masa: {topTable.Key} ({topTable.Count()} sipariş)\n";
            report += $"\n{new string('=', 60)}\n";

            // Event tetikle
            OnDailyReportGenerated(report);

            return report;
        }

        protected virtual void OnDailyReportGenerated(string report)
        {
            DailyReportGenerated?.Invoke(this, report);
        }

        // Tüm siparişleri getir
        public List<Order> GetAllOrders()
        {
            return _allOrders;
        }
    }
}

