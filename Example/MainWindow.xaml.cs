using System;
using System.Collections.Generic;
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
            Items.Add(Guid.NewGuid(), new TimelinerNet.TimelinerItem()
            {
                Name = "Item",
                Jobs = new Dictionary<Guid, TimelinerNet.TimelinerJob>()
                {
                    { Guid.NewGuid(), new TimelinerNet.TimelinerJob()
                        {
                            Name = "Job",
                            Begin = DateTime.Now - TimeSpan.FromMinutes(30),
                            End = DateTime.Now - TimeSpan.FromMinutes(15)
                        }
                    }
                }
            });
            DataContext = this;
            InitializeComponent();
        }
    }
}
