using System;
using System.Collections.Generic;
using System.Linq;

namespace RestoranSiparisYonetimi
{
    // Sipariş sınıfı - IOrder interface'ini implement eder
    public class Order : IOrder
    {
        private static int _orderCounter = 1;

        public int OrderId { get; private set; }
        public Table Table { get; set; }
        public List<Item> Items { get; private set; }
        public double TotalAmount { get; private set; }
        public DateTime OrderTime { get; private set; }

        public Order(Table table)
        {
            OrderId = _orderCounter++;
            Table = table;
            Items = new List<Item>();
            OrderTime = DateTime.Now;
            TotalAmount = 0;
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
            CalculateTotal();
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
            CalculateTotal();
        }

        public void CalculateTotal()
        {
            TotalAmount = Items.Sum(item => item.Subtotal);
        }

        public override string ToString()
        {
            return $"Sipariş #{OrderId} - {Table} - Toplam: {TotalAmount:C}";
        }
    }
}

