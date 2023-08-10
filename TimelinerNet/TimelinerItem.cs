using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TimelinerNet
{
    public class TimelinerItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public System.Windows.Controls.Viewbox Icon { get; private set; }
        public bool IsEnabled { get; set; }
        public ObservableCollection<TimelinerJob> Jobs { get; set; }
    }
}
