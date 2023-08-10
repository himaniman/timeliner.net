using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TimelinerNet;

namespace Example
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public int Test { get; set; }
        public List<TimelinerItem> Items { get; set; } = new();
        public MainWindow()
        {
            var now = new DateTime(2023, 08, 20, 23, 50, 00);
            Items.Add(new TimelinerNet.TimelinerItem()
            {
                Name = "BUS 33-8479\nSEAT 40",
                Jobs = new ()
                {
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "P/U GUIDE EO 2484",
                            TextUp = "Transfer",
                            Begin = now - TimeSpan.FromMinutes(30),
                            End = now - TimeSpan.FromMinutes(15),
                            Color = Brushes.LightSalmon,
                            CustomObject = new { CustomString = "Driver: MR. U THAI\n30 km" }
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "TROPICAL PARADISE(JUNGLE KINGDOM) (PHUKET)",
                            TextUp = "Excursion",
                            Begin = now - TimeSpan.FromMinutes(10),
                            End = now + TimeSpan.FromMinutes(10),
                            Color = Brushes.LightPink,
                            CustomObject = new { CustomString = "Driver: MR. DHET\n120 km" }
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "ALL DAY CITY TOUR",
                            TextUp = "Tour",
                            Begin = now + TimeSpan.FromMinutes(20),
                            End = now + TimeSpan.FromMinutes(40),
                            Color = Brushes.LightGreen,
                            CustomObject = new { CustomString = "Driver: MR. U THAI\n20 km" }
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "M",
                            TextUp = "Maintance",
                            Begin = now + TimeSpan.FromMinutes(70),
                            End = now + TimeSpan.FromMinutes(100),
                            Color = Brushes.LightYellow,
                            CustomObject = new { CustomString = "---" }
                        },
                    
                }
            });
            Items.Add(new TimelinerNet.TimelinerItem()
            {
                Name = "Test item",
                Jobs = new()
                {
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "JOB 1",
                            Begin = now - TimeSpan.FromMinutes(60),
                            End = now - TimeSpan.FromMinutes(50)
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "Micro",
                            Begin = now - TimeSpan.FromMinutes(48),
                            End = now - TimeSpan.FromMinutes(47)
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "Colored",
                            Begin = now - TimeSpan.FromMinutes(40),
                            End = now - TimeSpan.FromMinutes(30),
                            Color = Brushes.LightCoral,
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "Striped",
                            Begin = now - TimeSpan.FromMinutes(20),
                            End = now - TimeSpan.FromMinutes(10),
                            IsStripedColor = true,
                            Color = Brushes.LightPink,
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "Popup info",
                            Begin = now - TimeSpan.FromMinutes(9),
                            End = now - TimeSpan.FromMinutes(0),
                            CustomObject = new { CustomString = "Additional info" }
                        },
                }
            });
            DataContext = this;
            InitializeComponent();


            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    Items.First().Jobs.First().End += TimeSpan.FromMinutes(1);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
                    //Items = new List<TimelinerItem>(Items);
                    //Test++;
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Test)));
                }
            }).ContinueWith((t, o) => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
