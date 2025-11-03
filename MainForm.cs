using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace RestoranSiparisYonetimi
{
    public partial class MainForm : Form
    {
        private RestaurantManager manager;
        private FileManager fileManager;
        private Menu[] menuItems;
        private Table[] tables;
        private Order? currentOrder;
        
        private ListView menuListView;
        private ListBox orderItemsListBox;
        private ComboBox tableComboBox;
        private NumericUpDown quantityNumeric;
        private Label totalLabel;
        private Label orderCountLabel;
        private Label todayRevenueLabel;
        private Label avgOrderLabel;
        private Button addItemButton;
        private Button completeOrderButton;
        private Button clearOrderButton;
        private Button viewReportButton;
        private Button viewHistoryButton;
        private TextBox searchBox;
        private ComboBox categoryFilter;

        public MainForm()
        {
            InitializeComponent();
            InitializeData();
            SetupEventHandlers();
        }

        private void InitializeComponent()
        {
            this.Text = "🍽️ Restoran Sipariş Yönetim Sistemi";
            this.Size = new Size(1300, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.MinimumSize = new Size(1100, 650);

            // Ana TableLayoutPanel
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.FromArgb(240, 242, 245)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Başlık
            mainLayout.Controls.Add(CreateHeader(), 0, 0);
            
            // İstatistikler
            mainLayout.Controls.Add(CreateStats(), 0, 1);
            
            // İçerik (Menü ve Sipariş)
            mainLayout.Controls.Add(CreateContent(), 0, 2);

            this.Controls.Add(mainLayout);
        }

        private Panel CreateHeader()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(63, 81, 181)
            };

            var title = new Label
            {
                Text = "🍽️ RESTORAN SİPARİŞ YÖNETİM SİSTEMİ",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 25)
            };

            panel.Controls.Add(title);
            return panel;
        }

        private Panel CreateStats()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 242, 245),
                Padding = new Padding(15, 10, 15, 10)
            };

            var statsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            
            for (int i = 0; i < 4; i++)
                statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            // Bugünkü Siparişler
            var ordersCard = CreateStatCard("📦 Bugünkü Siparişler", "0", Color.FromArgb(76, 175, 80));
            orderCountLabel = ordersCard.Controls.OfType<Label>().Last();
            statsLayout.Controls.Add(ordersCard, 0, 0);

            // Bugünkü Ciro
            var revenueCard = CreateStatCard("💰 Bugünkü Ciro", "₺0,00", Color.FromArgb(255, 152, 0));
            todayRevenueLabel = revenueCard.Controls.OfType<Label>().Last();
            statsLayout.Controls.Add(revenueCard, 1, 0);

            // Ortalama Sipariş
            var avgCard = CreateStatCard("📊 Ortalama Sipariş", "₺0,00", Color.FromArgb(244, 67, 54));
            avgOrderLabel = avgCard.Controls.OfType<Label>().Last();
            statsLayout.Controls.Add(avgCard, 2, 0);

            // Toplam Masa
            statsLayout.Controls.Add(CreateStatCard("🪑 Toplam Masa", "6", Color.FromArgb(33, 150, 243)), 3, 0);

            panel.Controls.Add(statsLayout);
            return panel;
        }

        private Panel CreateStatCard(string title, string value, Color color)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5),
                Padding = new Padding(15)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(15, 10)
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = color,
                AutoSize = true,
                Location = new Point(15, 32)
            };

            card.Controls.AddRange(new Control[] { titleLabel, valueLabel });
            return card;
        }

        private TableLayoutPanel CreateContent()
        {
            var contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(15, 5, 15, 15),
                BackColor = Color.Transparent
            };
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));

            contentLayout.Controls.Add(CreateMenuPanel(), 0, 0);
            contentLayout.Controls.Add(CreateOrderPanel(), 1, 0);

            return contentLayout;
        }

        private Panel CreateMenuPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 5, 0)
            };

            // Ana layout
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.White
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));

            // Başlık
            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(103, 58, 183),
                Padding = new Padding(15, 10, 15, 10)
            };
            var headerTitle = new Label
            {
                Text = "📋 MENÜ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            header.Controls.Add(headerTitle);
            layout.Controls.Add(header, 0, 0);

            // Filtreler
            var filterPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 250),
                Padding = new Padding(10, 8, 10, 8)
            };

            categoryFilter = new ComboBox
            {
                Location = new Point(10, 10),
                Size = new Size(140, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            categoryFilter.Items.AddRange(new string[] { "Tümü", "Ana Yemek", "Çorba", "İçecek" });
            categoryFilter.SelectedIndex = 0;

            searchBox = new TextBox
            {
                Location = new Point(160, 10),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Text = "Ara..."
            };
            searchBox.GotFocus += (s, e) => { if (searchBox.Text == "Ara...") { searchBox.Text = ""; searchBox.ForeColor = Color.Black; } };
            searchBox.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(searchBox.Text)) { searchBox.Text = "Ara..."; searchBox.ForeColor = Color.Gray; } };
            searchBox.ForeColor = Color.Gray;

            filterPanel.Controls.AddRange(new Control[] { categoryFilter, searchBox });
            layout.Controls.Add(filterPanel, 0, 1);

            // Menü ListView
            menuListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(250, 250, 252),
                MultiSelect = false
            };
            menuListView.Columns.Add("Ürün Adı", 280);
            menuListView.Columns.Add("Fiyat", 100);
            layout.Controls.Add(menuListView, 0, 2);

            // Alt kısım - Ekleme
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 250),
                Padding = new Padding(10)
            };

            var qtyLabel = new Label
            {
                Text = "Adet:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 12),
                AutoSize = true
            };

            quantityNumeric = new NumericUpDown
            {
                Location = new Point(10, 32),
                Size = new Size(100, 25),
                Minimum = 1,
                Maximum = 99,
                Value = 1,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center
            };

            addItemButton = new Button
            {
                Text = "➕ SEPETE EKLE",
                Location = new Point(120, 12),
                Size = new Size(250, 45),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            addItemButton.FlatAppearance.BorderSize = 0;

            bottomPanel.Controls.AddRange(new Control[] { qtyLabel, quantityNumeric, addItemButton });
            layout.Controls.Add(bottomPanel, 0, 3);

            panel.Controls.Add(layout);
            return panel;
        }

        private Panel CreateOrderPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(5, 0, 0, 0)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.White
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));

            // Başlık
            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(255, 87, 34),
                Padding = new Padding(15, 10, 15, 10)
            };
            var headerTitle = new Label
            {
                Text = "🛒 SİPARİŞ DETAYI",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            header.Controls.Add(headerTitle);
            layout.Controls.Add(header, 0, 0);

            // Masa seçimi
            var tablePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 250),
                Padding = new Padding(10, 10, 10, 5)
            };
            var tableLabel = new Label
            {
                Text = "Masa Seçimi:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 5),
                AutoSize = true
            };
            tableComboBox = new ComboBox
            {
                Location = new Point(10, 25),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            tablePanel.Controls.AddRange(new Control[] { tableLabel, tableComboBox });
            layout.Controls.Add(tablePanel, 0, 1);

            // Sipariş listesi
            orderItemsListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(250, 250, 252),
                ItemHeight = 28
            };
            layout.Controls.Add(orderItemsListBox, 0, 2);

            // Toplam
            var totalPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15, 10, 15, 10)
            };
            var totalTitleLabel = new Label
            {
                Text = "TOPLAM:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(15, 5),
                AutoSize = true
            };
            totalLabel = new Label
            {
                Text = "₺0,00",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(211, 47, 47),
                Location = new Point(15, 25),
                AutoSize = true
            };
            totalPanel.Controls.AddRange(new Control[] { totalTitleLabel, totalLabel });
            layout.Controls.Add(totalPanel, 0, 3);

            // Butonlar
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 250),
                Padding = new Padding(10)
            };

            clearOrderButton = new Button
            {
                Text = "🗑️ TEMİZLE",
                Location = new Point(10, 10),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(245, 124, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            clearOrderButton.FlatAppearance.BorderSize = 0;

            completeOrderButton = new Button
            {
                Text = "✅ SİPARİŞİ TAMAMLA",
                Location = new Point(160, 10),
                Size = new Size(330, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            completeOrderButton.FlatAppearance.BorderSize = 0;

            viewReportButton = new Button
            {
                Text = "📊 GÜN SONU",
                Location = new Point(10, 52),
                Size = new Size(235, 30),
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            viewReportButton.FlatAppearance.BorderSize = 0;

            viewHistoryButton = new Button
            {
                Text = "📜 GEÇMİŞ",
                Location = new Point(255, 52),
                Size = new Size(235, 30),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            viewHistoryButton.FlatAppearance.BorderSize = 0;

            buttonPanel.Controls.AddRange(new Control[] 
            { 
                clearOrderButton, completeOrderButton, 
                viewReportButton, viewHistoryButton 
            });
            layout.Controls.Add(buttonPanel, 0, 4);

            panel.Controls.Add(layout);
            return panel;
        }

        private void InitializeData()
        {
            manager = new RestaurantManager();
            fileManager = new FileManager();
            menuItems = CreateMenu();
            tables = CreateTables();

            manager.NewOrderReceived += (sender, e) =>
            {
                fileManager.SaveOrder(e.Order);
                UpdateStats();
            };

            manager.DailyReportGenerated += (sender, report) =>
            {
                fileManager.SaveDailyReport(report, DateTime.Now);
            };

            LoadMenu();
            LoadTables();
            UpdateStats();
        }

        private void UpdateStats()
        {
            var todayOrders = manager.GetAllOrders()
                .Where(o => o.OrderTime.Date == DateTime.Now.Date).ToList();
            
            if (orderCountLabel != null)
                orderCountLabel.Text = todayOrders.Count.ToString();
            
            var totalRevenue = todayOrders.Sum(o => o.TotalAmount);
            if (todayRevenueLabel != null)
                todayRevenueLabel.Text = totalRevenue.ToString("C");

            var avgOrder = todayOrders.Count > 0 ? totalRevenue / todayOrders.Count : 0;
            if (avgOrderLabel != null)
                avgOrderLabel.Text = avgOrder.ToString("C");
        }

        private void LoadMenu()
        {
            menuListView.Items.Clear();
            
            var filtered = menuItems.AsEnumerable();
            
            if (categoryFilter?.SelectedIndex > 0)
            {
                var category = categoryFilter.SelectedItem.ToString();
                if (category == "İçecek")
                    filtered = filtered.Where(m => m is Beverage);
                else
                    filtered = filtered.Where(m => m is Food f && f.Category == category);
            }

            if (searchBox != null && searchBox.Text != "Ara..." && 
                !string.IsNullOrWhiteSpace(searchBox.Text))
            {
                filtered = filtered.Where(m => 
                    m.Name.ToLower().Contains(searchBox.Text.ToLower()));
            }

            foreach (var item in filtered)
            {
                var listItem = new ListViewItem(item.Name);
                listItem.SubItems.Add(item.Price.ToString("C"));
                listItem.Tag = item;
                menuListView.Items.Add(listItem);
            }
        }

        private void LoadTables()
        {
            tableComboBox.Items.Clear();
            foreach (var table in tables)
            {
                var status = table.IsOccupied ? "🔴 DOLU" : "🟢 BOŞ";
                tableComboBox.Items.Add(
                    $"Masa {table.TableNumber} ({table.Capacity} kişi) - {status}");
            }
            if (tableComboBox.Items.Count > 0)
                tableComboBox.SelectedIndex = 0;
        }

        private void SetupEventHandlers()
        {
            addItemButton.Click += AddItemButton_Click;
            completeOrderButton.Click += CompleteOrderButton_Click;
            clearOrderButton.Click += ClearOrderButton_Click;
            viewReportButton.Click += ViewReportButton_Click;
            viewHistoryButton.Click += ViewHistoryButton_Click;
            tableComboBox.SelectedIndexChanged += (s, e) =>
            {
                if (currentOrder == null && tableComboBox.SelectedIndex >= 0)
                {
                    currentOrder = new Order(tables[tableComboBox.SelectedIndex]);
                }
            };
            searchBox.TextChanged += (s, e) => LoadMenu();
            categoryFilter.SelectedIndexChanged += (s, e) => LoadMenu();
        }

        private void AddItemButton_Click(object? sender, EventArgs e)
        {
            if (menuListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("⚠️ Lütfen menüden bir ürün seçin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (currentOrder == null)
            {
                currentOrder = new Order(tables[tableComboBox.SelectedIndex]);
            }

            var selectedItem = menuListView.SelectedItems[0].Tag as Menu;
            if (selectedItem == null) return;

            int quantity = (int)quantityNumeric.Value;
            currentOrder.AddItem(new Item(selectedItem, quantity));
            UpdateOrderDisplay();

            MessageBox.Show($"✅ {selectedItem.Name} x {quantity} sepete eklendi!",
                "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateOrderDisplay()
        {
            orderItemsListBox.Items.Clear();
            
            if (currentOrder != null && currentOrder.Items.Count > 0)
            {
                foreach (var item in currentOrder.Items)
                {
                    orderItemsListBox.Items.Add(
                        $"{item.MenuItem.Name,-30} x{item.Quantity,2}  {item.Subtotal,10:C}");
                }
                totalLabel.Text = $"{currentOrder.TotalAmount:C}";
            }
            else
            {
                totalLabel.Text = "₺0,00";
            }
        }

        private void CompleteOrderButton_Click(object? sender, EventArgs e)
        {
            if (currentOrder == null || currentOrder.Items.Count == 0)
            {
                MessageBox.Show("⚠️ Sipariş sepeti boş!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"📋 Sipariş Özeti:\n\n" +
                $"Masa: {currentOrder.Table.TableNumber}\n" +
                $"Ürün Sayısı: {currentOrder.Items.Count}\n" +
                $"Toplam Tutar: {currentOrder.TotalAmount:C}\n\n" +
                $"Siparişi onaylıyor musunuz?",
                "✅ Sipariş Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                manager.AddOrder(currentOrder);
                MessageBox.Show(
                    $"🎉 Sipariş #{currentOrder.OrderId} başarıyla kaydedildi!\n\n" +
                    $"Masa: {currentOrder.Table.TableNumber}\n" +
                    $"Toplam: {currentOrder.TotalAmount:C}",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                
                currentOrder = null;
                UpdateOrderDisplay();
            }
        }

        private void ClearOrderButton_Click(object? sender, EventArgs e)
        {
            if (currentOrder != null && currentOrder.Items.Count > 0)
            {
                var result = MessageBox.Show(
                    "Sipariş sepetini temizlemek istediğinizden emin misiniz?",
                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.No) return;
            }

            currentOrder = null;
            UpdateOrderDisplay();
        }

        private void ViewReportButton_Click(object? sender, EventArgs e)
        {
            var reportForm = new ReportForm(manager, DateTime.Now.Date);
            reportForm.ShowDialog(this);
        }

        private void ViewHistoryButton_Click(object? sender, EventArgs e)
        {
            var orders = manager.GetAllOrders();
            
            var historyForm = new Form
            {
                Text = "📜 Sipariş Geçmişi",
                Size = new Size(900, 600),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White
            };

            var listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 10)
            };

            listView.Columns.Add("Sipariş No", 100);
            listView.Columns.Add("Masa", 100);
            listView.Columns.Add("Tarih & Saat", 170);
            listView.Columns.Add("Ürün Sayısı", 110);
            listView.Columns.Add("Toplam", 140);

            foreach (var order in orders.OrderByDescending(o => o.OrderTime))
            {
                var item = new ListViewItem($"#{order.OrderId}");
                item.SubItems.Add($"Masa {order.Table.TableNumber}");
                item.SubItems.Add(order.OrderTime.ToString("dd/MM/yyyy HH:mm:ss"));
                item.SubItems.Add($"{order.Items.Count} ürün");
                item.SubItems.Add(order.TotalAmount.ToString("C"));
                listView.Items.Add(item);
            }

            var summaryPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.FromArgb(33, 150, 243)
            };

            var summaryLabel = new Label
            {
                Text = $"Toplam {orders.Count} sipariş  |  Toplam Ciro: {orders.Sum(o => o.TotalAmount):C}",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White
            };

            summaryPanel.Controls.Add(summaryLabel);
            historyForm.Controls.Add(listView);
            historyForm.Controls.Add(summaryPanel);
            historyForm.ShowDialog(this);
        }

        private Menu[] CreateMenu()
        {
            return new Menu[]
            {
                new Food(1, "İskender", "Kuzu etli iskender", 150.00, "Ana Yemek"),
                new Food(2, "Döner", "Tavuk döner", 85.00, "Ana Yemek"),
                new Food(3, "Lahmacun", "Lahmacun", 35.00, "Ana Yemek"),
                new Food(4, "Pide", "Kıymalı pide", 95.00, "Ana Yemek"),
                new Food(5, "Köfte", "Izgara köfte", 120.00, "Ana Yemek"),
                new Food(6, "Tavuk Şiş", "Izgara tavuk", 110.00, "Ana Yemek"),
                new Food(7, "Adana Kebap", "Adana kebap", 130.00, "Ana Yemek"),
                new Food(8, "Mercimek Çorbası", "Mercimek çorbası", 25.00, "Çorba"),
                new Food(9, "Ezogelin Çorbası", "Ezogelin çorbası", 30.00, "Çorba"),
                new Food(10, "Yayla Çorbası", "Yayla çorbası", 28.00, "Çorba"),
                new Beverage(11, "Kola", "Kola", 15.00, "Soğuk"),
                new Beverage(12, "Fanta", "Fanta", 15.00, "Soğuk"),
                new Beverage(13, "Sprite", "Sprite", 15.00, "Soğuk"),
                new Beverage(14, "Su", "Su", 5.00, "Soğuk"),
                new Beverage(15, "Ayran", "Ayran", 12.00, "Soğuk"),
                new Beverage(16, "Çay", "Çay", 10.00, "Sıcak"),
                new Beverage(17, "Kahve", "Türk kahvesi", 25.00, "Sıcak"),
                new Beverage(18, "Nescafe", "Nescafe", 20.00, "Sıcak")
            };
        }

        private Table[] CreateTables()
        {
            return new Table[]
            {
                new Table(1, 4),
                new Table(2, 2),
                new Table(3, 6),
                new Table(4, 4),
                new Table(5, 2),
                new Table(6, 8)
            };
        }
    }
}
