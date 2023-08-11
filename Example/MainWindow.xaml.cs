using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        public TimelinerData Data { get; set; } = new();
        public DateTime Now { get; set; }
        public MainWindow()
        {
            Now = new DateTime(2023, 08, 20, 23, 50, 00);
            Data.Items.Add(new TimelinerNet.TimelinerItem()
            {
                Name = "BUS 33-8479\nSEAT 40",
                Jobs = new ()
                {
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "P/U GUIDE EO 2484",
                            TextUp = "Transfer",
                            Begin = Now - TimeSpan.FromMinutes(30),
                            End = Now - TimeSpan.FromMinutes(15),
                            Color = Brushes.LightSalmon,
                            CustomObject = new { CustomString = "Driver: MR. U THAI\n30 km" }
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "TROPICAL PARADISE(JUNGLE KINGDOM) (PHUKET)",
                            TextUp = "Excursion",
                            Begin = Now - TimeSpan.FromMinutes(10),
                            End = Now + TimeSpan.FromMinutes(10),
                            Color = Brushes.LightPink,
                            CustomObject = new { CustomString = "Driver: MR. DHET\n120 km" }
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "ALL DAY CITY TOUR",
                            TextUp = "Tour",
                            Begin = Now + TimeSpan.FromMinutes(20),
                            End = Now + TimeSpan.FromMinutes(40),
                            Color = Brushes.LightGreen,
                            CustomObject = new { CustomString = "Driver: MR. U THAI\n20 km" }
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "M",
                            TextUp = "Maintance",
                            Begin = Now + TimeSpan.FromMinutes(70),
                            End = Now + TimeSpan.FromMinutes(100),
                            Color = Brushes.LightYellow,
                            CustomObject = new { CustomString = "---" }
                        },
                    
                }
            });
            Data.Items.Add(new TimelinerNet.TimelinerItem()
            {
                Name = "Test item",
                Jobs = new ()
                {
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "JOB 1",
                            Begin = Now - TimeSpan.FromMinutes(60),
                            End = Now - TimeSpan.FromMinutes(50)
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "Micro",
                            Begin = Now - TimeSpan.FromMinutes(48),
                            End = Now - TimeSpan.FromMinutes(47)
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "Colored",
                            Begin = Now - TimeSpan.FromMinutes(40),
                            End = Now - TimeSpan.FromMinutes(30),
                            Color = Brushes.LightCoral,
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "Striped",
                            Begin = Now - TimeSpan.FromMinutes(20),
                            End = Now - TimeSpan.FromMinutes(10),
                            IsStripedColor = true,
                            Color = Brushes.LightPink,
                        },
                    
                    new TimelinerNet.TimelinerJob()
                        {
                            Name = "Popup info",
                            Begin = Now - TimeSpan.FromMinutes(9),
                            End = Now - TimeSpan.FromMinutes(0),
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
                    Now += TimeSpan.FromMinutes(1);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Now)));

                    Data = new();
                    Data.Items.Add(new TimelinerNet.TimelinerItem()
                    {
                        Jobs = new()
                        {
                            new TimelinerNet.TimelinerJob()
                                {
                                    Name = "P/U GUIDE EO 2484",
                                    TextUp = "Transfer",
                                    Begin = Now - TimeSpan.FromMinutes(30),
                                    End = Now - TimeSpan.FromMinutes(15),
                                    Color = Brushes.LightSalmon,
                                    CustomObject = new { CustomString = "Driver: MR. U THAI\n30 km" }
                                },
                        }
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
                }
            }).ContinueWith((t, o) => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
