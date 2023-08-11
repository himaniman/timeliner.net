using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace TimelinerNet
{
    public class TimelinerItem : INotifyPropertyChanged//, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        //public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public string Name { get; set; }
        public string Description { get; set; }
        public System.Windows.Controls.Viewbox Icon { get; set; }
        public bool IsEnabled { get; set; }
        public ICollection<TimelinerJob> Jobs { get; set; }
    }
}
