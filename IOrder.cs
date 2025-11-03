using System.Collections.Generic;

namespace RestoranSiparisYonetimi
{
    // Sipariş davranışlarını belirleyen interface
    public interface IOrder
    {
        int OrderId { get; }
        Table Table { get; }
        List<Item> Items { get; }
        double TotalAmount { get; }
        DateTime OrderTime { get; }

        void AddItem(Item item);
        void RemoveItem(Item item);
        void CalculateTotal();
    }
}

