using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RestoranSiparisYonetimi
{
    public class ReportForm : Form
    {
        private RestaurantManager manager;
        private DateTime reportDate;

        public ReportForm(RestaurantManager manager, DateTime date)
        {
            this.manager = manager;
            this.reportDate = date;
            InitializeComponent();
            LoadReport();
        }

        private void InitializeComponent()
        {
            this.Text = $"📊 Gün Sonu Raporu - {reportDate:dd MMMM yyyy}";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.MinimumSize = new Size(900, 600);

            // Ana scroll panel
            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 242, 245),
                Padding = new Padding(20)
            };

            this.Controls.Add(scrollPanel);
        }

        private void LoadReport()
        {
            var dailyOrders = manager.GetAllOrders()
                .Where(o => o.OrderTime.Date == reportDate.Date).ToList();

            var scrollPanel = this.Controls[0] as Panel;
            if (scrollPanel == null) return;

            int yPos = 10;

            if (!dailyOrders.Any())
            {
                var noDataLabel = new Label
                {
                    Text = "Bu tarihte hiç sipariş bulunamadı.",
                    Font = new Font("Segoe UI", 14),
                    ForeColor = Color.Gray,
                    Location = new Point(20, 50),
                    AutoSize = true
                };
                scrollPanel.Controls.Add(noDataLabel);
                return;
            }

            // Başlık
            var titlePanel = CreateTitlePanel();
            titlePanel.Location = new Point(10, yPos);
            scrollPanel.Controls.Add(titlePanel);
            yPos += titlePanel.Height + 15;

            // İstatistikler
            var statsPanel = CreateStatsPanel(dailyOrders);
            statsPanel.Location = new Point(10, yPos);
            scrollPanel.Controls.Add(statsPanel);
            yPos += statsPanel.Height + 15;

            // Masa ve Zaman Analizi
            var analysisPanel = CreateAnalysisPanel(dailyOrders);
            analysisPanel.Location = new Point(10, yPos);
            scrollPanel.Controls.Add(analysisPanel);
            yPos += analysisPanel.Height + 15;

            // En Çok Satan Ürünler
            var productsPanel = CreateTopProductsPanel(dailyOrders);
            productsPanel.Location = new Point(10, yPos);
            scrollPanel.Controls.Add(productsPanel);
            yPos += productsPanel.Height + 15;

            // Kategori Analizi
            var categoryPanel = CreateCategoryPanel(dailyOrders);
            categoryPanel.Location = new Point(10, yPos);
            scrollPanel.Controls.Add(categoryPanel);
        }

        private Panel CreateTitlePanel()
        {
            var panel = new Panel
            {
                Size = new Size(1040, 100),
                BackColor = Color.FromArgb(63, 81, 181)
            };

            var titleLabel = new Label
            {
                Text = "📊 GÜN SONU SATIŞ RAPORU",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 20),
                AutoSize = true
            };

            var dateLabel = new Label
            {
                Text = reportDate.ToString("dd MMMM yyyy, dddd"),
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(200, 220, 255),
                Location = new Point(30, 60),
                AutoSize = true
            };

            panel.Controls.AddRange(new Control[] { titleLabel, dateLabel });
            return panel;
        }

        private Panel CreateStatsPanel(System.Collections.Generic.List<Order> orders)
        {
            var panel = new Panel
            {
                Size = new Size(1040, 180),
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = "📈 GENEL İSTATİSTİKLER",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 15),
                AutoSize = true
            };
            panel.Controls.Add(titleLabel);

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var averageOrder = orders.Average(o => o.TotalAmount);
            var totalItemsSold = orders.Sum(o => o.Items.Sum(i => i.Quantity));

            // İstatistik kartları
            var card1 = CreateStatCard("💰 Toplam Ciro", totalRevenue.ToString("C"), 
                Color.FromArgb(76, 175, 80), 20, 60);
            var card2 = CreateStatCard("📦 Toplam Sipariş", totalOrders.ToString(), 
                Color.FromArgb(33, 150, 243), 270, 60);
            var card3 = CreateStatCard("📊 Ortalama Sipariş", averageOrder.ToString("C"), 
                Color.FromArgb(255, 152, 0), 520, 60);
            var card4 = CreateStatCard("🛒 Satılan Ürün", totalItemsSold.ToString(), 
                Color.FromArgb(156, 39, 176), 770, 60);

            panel.Controls.AddRange(new Control[] { card1, card2, card3, card4 });
            return panel;
        }

        private Panel CreateStatCard(string title, string value, Color color, int x, int y)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(230, 100),
                BackColor = color
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 45),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            card.Controls.AddRange(new Control[] { titleLabel, valueLabel });
            return card;
        }

        private Panel CreateAnalysisPanel(System.Collections.Generic.List<Order> orders)
        {
            var panel = new Panel
            {
                Size = new Size(1040, 200),
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = "🔍 DETAYLI ANALİZ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 15),
                AutoSize = true
            };
            panel.Controls.Add(titleLabel);

            // Masa Analizi
            var topTable = orders
                .GroupBy(o => o.Table.TableNumber)
                .Select(g => new { TableNumber = g.Key, Count = g.Count(), Revenue = g.Sum(o => o.TotalAmount) })
                .OrderByDescending(x => x.Count)
                .First();

            var tablePanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(490, 120),
                BackColor = Color.FromArgb(245, 245, 250)
            };

            var tableTitle = new Label
            {
                Text = "🏆 En Çok Sipariş Alan Masa",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                Location = new Point(15, 15),
                AutoSize = true
            };

            var tableInfo = new Label
            {
                Text = $"Masa {topTable.TableNumber}\n{topTable.Count} sipariş  •  {topTable.Revenue:C}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(76, 175, 80),
                Location = new Point(15, 50),
                AutoSize = true
            };

            tablePanel.Controls.AddRange(new Control[] { tableTitle, tableInfo });
            panel.Controls.Add(tablePanel);

            // Zaman Analizi
            var peakHour = orders
                .GroupBy(o => o.OrderTime.Hour)
                .Select(g => new { Hour = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            if (peakHour != null)
            {
                var timePanel = new Panel
                {
                    Location = new Point(530, 60),
                    Size = new Size(490, 120),
                    BackColor = Color.FromArgb(245, 245, 250)
                };

                var timeTitle = new Label
                {
                    Text = "🕐 En Yoğun Saat",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(66, 66, 66),
                    Location = new Point(15, 15),
                    AutoSize = true
                };

                var timeInfo = new Label
                {
                    Text = $"{peakHour.Hour:00}:00\n{peakHour.Count} sipariş",
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = Color.FromArgb(33, 150, 243),
                    Location = new Point(15, 50),
                    AutoSize = true
                };

                timePanel.Controls.AddRange(new Control[] { timeTitle, timeInfo });
                panel.Controls.Add(timePanel);
            }

            return panel;
        }

        private Panel CreateTopProductsPanel(System.Collections.Generic.List<Order> orders)
        {
            var panel = new Panel
            {
                Size = new Size(1040, 350),
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = "🥇 EN ÇOK SATAN ÜRÜNLER",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 15),
                AutoSize = true
            };
            panel.Controls.Add(titleLabel);

            var topProducts = orders
                .SelectMany(o => o.Items)
                .GroupBy(i => i.MenuItem.Name)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(i => i.Quantity),
                    TotalRevenue = g.Sum(i => i.Subtotal)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToList();

            var listView = new ListView
            {
                Location = new Point(20, 60),
                Size = new Size(1000, 270),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None
            };

            listView.Columns.Add("Sıra", 60);
            listView.Columns.Add("Ürün Adı", 500);
            listView.Columns.Add("Satış Adedi", 150);
            listView.Columns.Add("Toplam Ciro", 180);

            int rank = 1;
            foreach (var product in topProducts)
            {
                string medal = rank == 1 ? "🥇" : rank == 2 ? "🥈" : rank == 3 ? "🥉" : $"{rank}.";
                var item = new ListViewItem(medal);
                item.SubItems.Add(product.ProductName);
                item.SubItems.Add(product.TotalQuantity.ToString());
                item.SubItems.Add(product.TotalRevenue.ToString("C"));
                
                if (rank <= 3)
                {
                    item.BackColor = rank == 1 ? Color.FromArgb(255, 248, 225) :
                                    rank == 2 ? Color.FromArgb(245, 245, 245) :
                                    Color.FromArgb(255, 243, 224);
                }
                
                listView.Items.Add(item);
                rank++;
            }

            panel.Controls.Add(listView);
            return panel;
        }

        private Panel CreateCategoryPanel(System.Collections.Generic.List<Order> orders)
        {
            var panel = new Panel
            {
                Size = new Size(1040, 350),
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = "📊 KATEGORİ BAZLI SATIŞ ANALİZİ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 15),
                AutoSize = true
            };
            panel.Controls.Add(titleLabel);

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var categoryAnalysis = orders
                .SelectMany(o => o.Items)
                .GroupBy(i => i.MenuItem is Food ? ((Food)i.MenuItem).Category : "İçecek")
                .Select(g => new
                {
                    Category = g.Key,
                    TotalQuantity = g.Sum(i => i.Quantity),
                    TotalRevenue = g.Sum(i => i.Subtotal)
                })
                .Select(x => new
                {
                    x.Category,
                    x.TotalQuantity,
                    x.TotalRevenue,
                    Percentage = (x.TotalRevenue / totalRevenue) * 100
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            int yPos = 70;
            foreach (var category in categoryAnalysis)
            {
                var categoryCard = new Panel
                {
                    Location = new Point(20, yPos),
                    Size = new Size(1000, 80),
                    BackColor = Color.FromArgb(245, 245, 250)
                };

                string emoji = category.Category == "Ana Yemek" ? "🍖" :
                              category.Category == "Çorba" ? "🍲" : "🥤";

                var catLabel = new Label
                {
                    Text = $"{emoji} {category.Category}",
                    Font = new Font("Segoe UI", 13, FontStyle.Bold),
                    ForeColor = Color.FromArgb(33, 33, 33),
                    Location = new Point(20, 15),
                    AutoSize = true
                };

                var statsLabel = new Label
                {
                    Text = $"{category.TotalQuantity} adet  •  {category.TotalRevenue:C}  •  {category.Percentage:F1}%",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(100, 100, 100),
                    Location = new Point(20, 45),
                    AutoSize = true
                };

                // Progress bar
                var progressBack = new Panel
                {
                    Location = new Point(400, 35),
                    Size = new Size(580, 30),
                    BackColor = Color.FromArgb(220, 220, 220)
                };

                var progressFill = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size((int)(580 * (category.Percentage / 100)), 30),
                    BackColor = category.Category == "Ana Yemek" ? Color.FromArgb(76, 175, 80) :
                               category.Category == "Çorba" ? Color.FromArgb(255, 152, 0) :
                               Color.FromArgb(33, 150, 243)
                };

                var percentLabel = new Label
                {
                    Text = $"{category.Percentage:F1}%",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    BackColor = Color.Transparent
                };
                percentLabel.Location = new Point(
                    (progressFill.Width - percentLabel.Width) / 2,
                    (progressFill.Height - percentLabel.Height) / 2
                );

                progressFill.Controls.Add(percentLabel);
                progressBack.Controls.Add(progressFill);
                categoryCard.Controls.AddRange(new Control[] { catLabel, statsLabel, progressBack });
                panel.Controls.Add(categoryCard);

                yPos += 90;
            }

            panel.Height = yPos + 20;
            return panel;
        }
    }
}

