using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static TimelinerNet.SpanMode;
using static TimelinerNet.Stuff;

namespace TimelinerNet
{
    public partial class Timeliner : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Point initMousePoint;
        private DateTime initCaptureLeftEdge;
        private DateTime initCaptureRightEdge;
        private TimeSpan initCaptureScalePx;
        private Line NowMarker1;
        private Line NowMarker2;
        private CultureInfo cultureInfo = CultureInfo.GetCultureInfo("en-EN");
        public bool IsOnManipulate { get; private set; }
        public bool IsNeedSidePanel => Data?.IsNeedSidePanel ?? true;

        public TimelinerData Data
        {
            get { return (TimelinerData)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(TimelinerData), typeof(Timeliner), new PropertyMetadata(null, new PropertyChangedCallback(DataPropertyChangedCallback)));

        private static void DataPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Timeliner)
            {
                Timeliner input = (Timeliner)d;
                if (e.NewValue != null && e.NewValue is TimelinerData)
                {
                    input.PropertyChanged?.Invoke(input, new PropertyChangedEventArgs(nameof(IsNeedSidePanel)));
                }
                input.RedrawGrid();
            }
        }

        public DateTime Now
        {
            get { return (DateTime)GetValue(NowProperty); }
            set { SetValue(NowProperty, value); }
        }

        public static readonly DependencyProperty NowProperty =
            DependencyProperty.Register("Now", typeof(DateTime), typeof(Timeliner), new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(NowPropertyChangedCallback)));

        private static void NowPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Timeliner)
            {
                Timeliner input = (Timeliner)d;
                if (e.NewValue != null && e.NewValue is DateTime)
                {
                    if (input.TrackNow && e.OldValue != null && e.OldValue != default && e.NewValue != default)
                    {
                        input.LeftEdge += (DateTime)e.NewValue - (DateTime)e.OldValue;
                        input.RightEdge += (DateTime)e.NewValue - (DateTime)e.OldValue;
                        input.RedrawGrid();
                    }
                }
                input.RedrawNowMarker();
            }
        }



        public DateTime LeftEdge
        {
            get { return (DateTime)GetValue(LeftEdgeProperty); }
            set { SetValue(LeftEdgeProperty, value); }
        }

        public static readonly DependencyProperty LeftEdgeProperty =
            DependencyProperty.Register("LeftEdge", typeof(DateTime), typeof(Timeliner), new PropertyMetadata(DateTime.Now - TimeSpan.FromMinutes(10), new PropertyChangedCallback(EdgePropertyChangedCallback)));

        public DateTime RightEdge
        {
            get { return (DateTime)GetValue(RightEdgeProperty); }
            set { SetValue(RightEdgeProperty, value); }
        }

        public static readonly DependencyProperty RightEdgeProperty =
            DependencyProperty.Register("RightEdge", typeof(DateTime), typeof(Timeliner), new PropertyMetadata(DateTime.Now + TimeSpan.FromMinutes(10), new PropertyChangedCallback(EdgePropertyChangedCallback)));

        private static void EdgePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Timeliner)
            {
                Timeliner input = (Timeliner)d;
                if (e.NewValue != null && e.NewValue is DateTime)
                {
                    
                }
                input.RedrawGrid();
            }
        }


        public bool TrackNow
        {
            get { return (bool)GetValue(TrackNowProperty); }
            set { SetValue(TrackNowProperty, value); }
        }

        public static readonly DependencyProperty TrackNowProperty =
            DependencyProperty.Register("TrackNow", typeof(bool), typeof(Timeliner), new PropertyMetadata(false));


        public DataTemplate DataTemplatePopup
        {
            get { return (DataTemplate)GetValue(DataTemplatePopupProperty); }
            set { SetValue(DataTemplatePopupProperty, value); }
        }

        public static readonly DependencyProperty DataTemplatePopupProperty =
            DependencyProperty.Register("DataTemplatePopup", typeof(DataTemplate), typeof(Timeliner), new PropertyMetadata(null));


        public Timeliner()
        {
            InitializeComponent();
        }

        private void previewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Mouse.Capture(sender as IInputElement) && !popup_info.IsOpen)
            {
                initMousePoint = e.GetPosition(sender as IInputElement);
                initCaptureLeftEdge = LeftEdge;
                initCaptureRightEdge = RightEdge;
                initCaptureScalePx = (RightEdge - LeftEdge) / grid_Timeline.ActualWidth;
                IsOnManipulate = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOnManipulate)));
                //e.Handled = true;

            }
        }

        private void previewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                IsOnManipulate = false;
                //popup_info.IsOpen = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOnManipulate)));
                Mouse.Capture(null);
                e.Handled = true;
            }
        }

        private void previewMouseMove(object sender, MouseEventArgs e)
        {
            if (IsOnManipulate && e.LeftButton == MouseButtonState.Pressed && !popup_info.IsOpen)
            {
                double deltapx = initMousePoint.X - e.GetPosition(sender as IInputElement).X;
                LeftEdge = initCaptureLeftEdge + deltapx * initCaptureScalePx;
                RightEdge = initCaptureRightEdge + deltapx * initCaptureScalePx;
                RedrawGrid();
                e.Handled = true;
            }
        }

        private void previewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {

            }
            else
            {
                var xSize = grid_Timeline.ActualWidth;
                var posx = e.GetPosition(sender as IInputElement).X;
                var weight = 0.5;
                if (posx >= 0 && posx < xSize)
                {
                    weight = posx / xSize;
                }

                var span = RightEdge - LeftEdge;
                if (e.Delta < 0)
                {
                    LeftEdge -= span * 0.05 * weight;
                    RightEdge += span * 0.05 * (1 - weight);
                }
                else
                {
                    LeftEdge += span * 0.05 * weight;
                    RightEdge -= span * 0.05 * (1 - weight);
                }
            }
            RedrawGrid();
            e.Handled = true;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            scrollViewer_MainData.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawGrid();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            RedrawGrid();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (popup_info.IsOpen)
            {
                Mouse.Capture(null);
                popup_info.IsOpen = false;
                IsOnManipulate = false;
                e.Handled = true;
            }
            else base.OnPreviewMouseDown(e);
        }
    }
}
