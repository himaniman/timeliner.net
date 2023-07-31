using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelinerNet
{
    public class TimelinerItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public System.Windows.Controls.Viewbox Icon { get; private set; }
        public bool IsEnabled { get; set; }
        public Dictionary <Guid, TimelinerJob> Jobs { get; set; }
    }
}
