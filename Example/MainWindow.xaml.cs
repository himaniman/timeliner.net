﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Example
{
    public partial class MainWindow : Window
    {
        public Dictionary<Guid, TimelinerNet.TimelinerItem> Items { get; private set; } = new Dictionary<Guid, TimelinerNet.TimelinerItem>();
        public MainWindow()
        {
            var now = new DateTime(2023, 08, 20, 23, 50, 00);
            Items.Add(Guid.NewGuid(), new TimelinerNet.TimelinerItem()
            {
                Name = "Item",
                Jobs = new Dictionary<Guid, TimelinerNet.TimelinerJob>()
                {
                    { Guid.NewGuid(), new TimelinerNet.TimelinerJob()
                        {
                            Name = "Job",
                            Begin = now - TimeSpan.FromMinutes(30),
                            End = now - TimeSpan.FromMinutes(15),
                            Color = Brushes.LightGreen,
                        }
                    },
                    { Guid.NewGuid(), new TimelinerNet.TimelinerJob()
                        {
                            Name = "Job2",
                            Begin = now - TimeSpan.FromMinutes(45),
                            End = now - TimeSpan.FromMinutes(35)
                        }
                    }
                }
            });
            Items.Add(Guid.NewGuid(), new TimelinerNet.TimelinerItem()
            {
                Name = "Item2",
                Jobs = new Dictionary<Guid, TimelinerNet.TimelinerJob>()
                {
                    { Guid.NewGuid(), new TimelinerNet.TimelinerJob()
                        {
                            Name = "Job123",
                            Begin = now - TimeSpan.FromMinutes(20),
                            End = now - TimeSpan.FromMinutes(18)
                        }
                    },
                    { Guid.NewGuid(), new TimelinerNet.TimelinerJob()
                        {
                            Name = "Job456",
                            Begin = now - TimeSpan.FromMinutes(10),
                            End = now + TimeSpan.FromMinutes(10)
                        }
                    }
                }
            });
            DataContext = this;
            InitializeComponent();
        }
    }
}
