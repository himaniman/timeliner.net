using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelinerNet
{
    public class TimelinerData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public IEnumerable<TimelinerItem> Items { get; set; }
    }
}
