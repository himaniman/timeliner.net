using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelinerNet
{
    public class TimelinerData
    {
        public bool IsNeedSidePanel => Items?.Count() > 0 && Items.Any(x => !string.IsNullOrEmpty(x.Name));
        public List<TimelinerItem> Items { get; set; } = new List<TimelinerItem>();
    }
}
