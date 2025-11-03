namespace RestoranSiparisYonetimi
{
    // Sipariş kalemi - Menu item ve miktar bilgisi
    public class Item
    {
        public Menu MenuItem { get; set; }
        public int Quantity { get; set; }
        public double Subtotal => MenuItem.Price * Quantity;

        public Item(Menu menuItem, int quantity)
        {
            MenuItem = menuItem;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{MenuItem.Name} x {Quantity} = {Subtotal:C}";
        }
    }
}

