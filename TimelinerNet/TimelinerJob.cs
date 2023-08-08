using System;
using System.Windows;
using System.Windows.Media;

namespace TimelinerNet
{
    public class TimelinerJob
    {
        public string Name { get; set; }
        public string TextUp { get; set; }
        public string TextDown { get; set; }
        public string TextRight { get; set; }
        public Brush Color { get; set; } = SystemColors.ActiveCaptionBrush;
        public bool IsStripedColor { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public object CustomObject { get; set; }
    }
}
