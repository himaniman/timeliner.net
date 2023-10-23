using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace TimelinerNet
{
    public class TimelinerItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public System.Windows.Controls.Viewbox Icon { get; set; }
        public bool IsEnabled { get; set; }
        public List<TimelinerJob> Jobs { get; set; } = new List<TimelinerJob>();
    }
}
