using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelinerNet
{
    public class TimelinerJob
    {
        public string Name { get; set; }
        public string TextUp { get; set; }
        public string TextDown { get; set; }
        public string TextRight { get; set; }
        public Color Color { get; set; }
        public bool IsCrossColor { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }
}
