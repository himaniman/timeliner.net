using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static TimelinerNet.SpanMode;
using static TimelinerNet.Stuff;

namespace TimelinerNet
{
    public partial class Timeliner : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DateTime Now;
        private DateTime LeftEdge;
        private DateTime RightEdge;

        public Dictionary<Guid, TimelinerItem> Items
        {
            get { return (Dictionary<Guid, TimelinerItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(Dictionary<Guid, TimelinerItem>), typeof(Timeliner), new PropertyMetadata(null, new PropertyChangedCallback(ItemsPropertyChangedCallback)));

        private static void ItemsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is Dictionary<Guid, TimelinerItem>)
            {

                //BreadcrumbPicker input = (BreadcrumbPicker)d;
                //input.ParseStringToPath();
                //input.SetPathFromHardLink(e.NewValue as string);
                //input.RedrawValue(AverOn: input.Averaging);
            }
        }

        public Timeliner()
        {
            Now = new DateTime(2023, 08, 20, 23, 50, 00);
            LeftEdge = Now - TimeSpan.FromMinutes(60);
            RightEdge = Now + TimeSpan.FromMinutes(30);
            InitializeComponent();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                //if (e.Delta < 0) OffsetValue(-1);
                //else OffsetValue(1);
                //initMousePoint = e.GetPosition(this as IInputElement);
                //initValue = Value;
            }
            else
            {
                var span = RightEdge - LeftEdge;
                if (e.Delta < 0)
                {
                    LeftEdge -= span * 0.1;
                    RightEdge += span * 0.1;
                }
                else
                {
                    LeftEdge += span * 0.1;
                    RightEdge -= span * 0.1;
                }

                //double newSpan;
                //if (SmoothSpan)
                //{
                //    if (e.Delta < 0) newSpan = Span * 1.1;
                //    else newSpan = Span / 1.1;
                //}
                //else
                //{
                //    if (e.Delta < 0) newSpan = Near125(Span * 2);
                //    else newSpan = Near125(Span / 2);
                //}
                //SetNewSpan(newSpan);
            }
            RedrawGrid();
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValueString)));
            e.Handled = true;
            base.OnPreviewMouseWheel(e);
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

        public void RedrawGrid()
        {
            var xSize = grid_Timeline.ActualWidth;
            grid_Timeline.Children.Clear();

            var span = RightEdge - LeftEdge;
            var majorMode = span.NearSpanMode();
            var minorMode = span.NearSpanMode() - 1;
            var major = majorMode.ModeToSpan();
            var minor = minorMode.ModeToSpan();
            var timePerPixcel = span / xSize;

            var leftEdgeMajor = LeftEdge.GetMajorLeftEdge(majorMode);
            var currentMajor = leftEdgeMajor;

            var subTicks = new List<UIElement>();


            while (currentMajor <= RightEdge)
            {
                var offset = currentMajor - leftEdgeMajor - (LeftEdge - leftEdgeMajor);
                var offsetPx = offset.ToPixcel(span, xSize);
                var widthPx = major.ToPixcel(span, xSize);


                grid_Timeline.Children.Add(new Border
                {
                    Width = widthPx,
                    Margin = new Thickness(offsetPx, 0, 0, 0),
                    Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xAC, 0xAC, 0xAC)),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00)),
                    BorderThickness = new Thickness(1,0,0,0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Child = new TextBlock
                    {
                        Text = currentMajor.ToString() + " " + offset.ToString(),
                        HorizontalAlignment = offsetPx < 0 ? HorizontalAlignment.Right : offsetPx + widthPx > xSize ? HorizontalAlignment.Left : HorizontalAlignment.Center,
                        Margin = new Thickness(2)
                    }

                    //X1 = px,
                    //Y1 = 6,
                    //X2 = px,
                    //Y2 = 6 + 5,
                    //Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xAC, 0xAC, 0xAC)),
                    //StrokeThickness = 1,
                    //SnapsToDevicePixels = true,
                });
                currentMajor += major;
            }

        }


        //private TimeSpan NearMajorSpan(TimeSpan span)
        //{
        //    if (span <= TimeSpan.FromSeconds(1)) return TimeSpan.FromMilliseconds(100);
        //    else if (span <= TimeSpan.FromMinutes(1)) return TimeSpan.FromSeconds(1);
        //    else if (span <= TimeSpan.FromHours(1)) return TimeSpan.FromMinutes(1);
        //    else if (span <= TimeSpan.FromDays(1)) return TimeSpan.FromHours(1);
        //    else if (span <= TimeSpan.FromDays(30)) return TimeSpan.FromDays(1);
        //    else if (span <= TimeSpan.FromDays(360)) return TimeSpan.FromDays(30);
        //    else return TimeSpan.FromDays(360);
        //}
    }
}
