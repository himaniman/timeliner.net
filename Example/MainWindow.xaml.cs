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
        public DateTime LeftEdge { get; set; }
        public DateTime RightEdge { get; set; }
        public MainWindow()
        {
            Now = new DateTime(2023, 08, 20, 23, 50, 00);
            LeftEdge = Now - TimeSpan.FromMinutes(60);
            RightEdge = Now + TimeSpan.FromMinutes(30);
            Data.Items.Add(new TimelinerItem()
            {
                Name = "Item #1\nSecond string and (fix dry)",
                Description = "Description tooltip\nHere",
                Jobs = new ()
                {
                    new TimelinerJob()
                        {
                            Name = "Job 1",
                            TextUp = "Text Up 1",
                            Begin = Now - TimeSpan.FromMinutes(30),
                            End = Now - TimeSpan.FromMinutes(15),
                            Color = Brushes.LightSalmon,
                            CustomObject = new { CustomString = "Custom string info 1 row\nCustom string info 2 row" }
                        },
                    
                    new TimelinerJob()
                        {
                            Name = "Job 2",
                            TextUp = "Text Up 2",
                            Begin = Now - TimeSpan.FromMinutes(10),
                            End = Now + TimeSpan.FromMinutes(10),
                            Color = Brushes.LightPink,
                            CustomObject = new { CustomString = "Custom string info 1 row\nCustom string info 2 row" }
                        },
                    
                    new TimelinerJob()
                        {
                            Name = "Job 3",
                            TextUp = "Text Up 3",
                            Begin = Now + TimeSpan.FromMinutes(20),
                            End = Now + TimeSpan.FromMinutes(40),
                            Color = Brushes.LightGreen,
                            CustomObject = new { CustomString = "Custom string info 1 row\nCustom string info 2 row" }
                        },
                    
                    new TimelinerJob()
                        {
                            Name = "Job 4",
                            TextUp = "Text Up 4",
                            Begin = Now + TimeSpan.FromMinutes(70),
                            End = Now + TimeSpan.FromMinutes(100),
                            Color = Brushes.LightYellow,
                            CustomObject = new { CustomString = "---" }
                        },
                    
                }
            });
            Data.Items.Add(new TimelinerItem()
            {
                Name = "Test item",
                Description = "Description tooltip\nHere",
                Jobs = new ()
                {
                    new TimelinerJob()
                        {
                            Name = "JOB 1",
                            Begin = Now - TimeSpan.FromMinutes(60),
                            End = Now - TimeSpan.FromMinutes(50)
                        },
                    
                    new TimelinerJob()
                        {
                            Name = "Micro",
                            Begin = Now - TimeSpan.FromMinutes(48),
                            End = Now - TimeSpan.FromMinutes(47)
                        },
                    
                    new TimelinerJob()
                        {
                            Name = "Colored",
                            Begin = Now - TimeSpan.FromMinutes(40),
                            End = Now - TimeSpan.FromMinutes(30),
                            Color = Brushes.LightCoral,
                        },
                    
                    new TimelinerJob()
                        {
                            Name = "Striped",
                            Begin = Now - TimeSpan.FromMinutes(20),
                            End = Now - TimeSpan.FromMinutes(10),
                            IsStripedColor = true,
                            Color = Brushes.LightPink,
                        },
                    
                    new TimelinerJob()
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
        }

        private void comboBox_FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LeftEdge -= TimeSpan.FromSeconds(10);
            RightEdge += TimeSpan.FromSeconds(10);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LeftEdge)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RightEdge)));
        }
    }
}
