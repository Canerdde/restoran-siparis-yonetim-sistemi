namespace RestoranSiparisYonetimi
{
    // Menü öğelerini tutan base class
    public abstract class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        protected Menu(int id, string name, string description, double price)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
        }

        public abstract void DisplayInfo();
    }

    // Yiyecek sınıfı
    public class Food : Menu
    {
        public string Category { get; set; } // "Ana Yemek", "Salata", "Çorba" vb.

        public Food(int id, string name, string description, double price, string category)
            : base(id, name, description, price)
        {
            Category = category;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Yiyecek] {Name} - {Category} - {Price:C}");
        }
    }

    // İçecek sınıfı
    public class Beverage : Menu
    {
        public string Type { get; set; } // "Sıcak", "Soğuk"

        public Beverage(int id, string name, string description, double price, string type)
            : base(id, name, description, price)
        {
            Type = type;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[İçecek] {Name} - {Type} - {Price:C}");
        }
    }
}

