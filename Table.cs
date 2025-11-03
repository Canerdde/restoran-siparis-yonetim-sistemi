namespace RestoranSiparisYonetimi
{
    // Masa bilgilerini tutan class
    public class Table
    {
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public bool IsOccupied { get; set; }

        public Table(int tableNumber, int capacity)
        {
            TableNumber = tableNumber;
            Capacity = capacity;
            IsOccupied = false;
        }

        public override string ToString()
        {
            return $"Masa {TableNumber} (Kapasite: {Capacity})";
        }
    }
}

