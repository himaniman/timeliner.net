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
        private Point initMousePoint;
        private DateTime mouseCapturePoint;

        public bool IsOnManipulate { get; set; }

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

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && Mouse.Capture(this as IInputElement))
            {
                initMousePoint = e.GetPosition(this as IInputElement);
                mouseCapturePoint = LeftEdge;
                IsOnManipulate = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOnManipulate)));
                e.Handled = true;
            }
            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                IsOnManipulate = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOnManipulate)));
                Mouse.Capture(null);
                e.Handled = true;
            }
            base.OnPreviewMouseUp(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (IsOnManipulate && e.LeftButton == MouseButtonState.Pressed)
            {
                double deltapx = initMousePoint.X - e.GetPosition(this as IInputElement).X;

                var span = RightEdge - LeftEdge;
                LeftEdge += span * 0.00001 * deltapx;
                RightEdge += span * 0.00001 * deltapx; 
                RedrawGrid();
                e.Handled = true;
            }
            base.OnPreviewMouseMove(e);
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
            grid_MainGrid.Children.Clear();

            var span = RightEdge - LeftEdge;
            var majorMode = span.NearSpanMode();
            var minorMode = span.NearSpanMode() - 1;
            var major = majorMode.ModeToSpan();
            var minor = minorMode.ModeToSpan();
            var timePerPixcel = span / xSize;
            var majorWithPx = major.ToPixcel(span, xSize);

            var leftEdgeMajor = LeftEdge.GetMajorLeftEdge(majorMode);
            var currentMajor = leftEdgeMajor;

            var subTicks = new List<TextBlock>();
            var subTicksCnt = major / minor;
            var subTickWidth = majorWithPx / subTicksCnt;
            for (int t = 0; t < subTicksCnt; t++)
            {
                subTicks.Add(new TextBlock
                {
                    Text = t.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    TextAlignment = TextAlignment.Center,
                    Width = subTickWidth,
                    Margin = new Thickness(subTickWidth * t, 2, 2, 2)
                });
            }

            while (currentMajor <= RightEdge)
            {
                var offset = currentMajor - leftEdgeMajor - (LeftEdge - leftEdgeMajor);
                var offsetPx = offset.ToPixcel(span, xSize);

                var border = new Border
                {
                    Width = majorWithPx,
                    Margin = new Thickness(offsetPx, 0, 0, 0),
                    Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xAC, 0xAC, 0xAC)),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00)),
                    BorderThickness = new Thickness(1, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Child = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = currentMajor.ToString() + " " + offset.ToString(),
                                HorizontalAlignment = offsetPx < 0 ? HorizontalAlignment.Right : offsetPx + majorWithPx > xSize ? HorizontalAlignment.Left : HorizontalAlignment.Center,
                                Margin = new Thickness(2)
                            },
                            new Grid
                            {
                                ClipToBounds = true,
                            }
                        }
                    }
                };
                subTicks.ForEach(x => ((border.Child as StackPanel).Children[1] as Grid).Children.Add(x.GetCopy()));
                grid_Timeline.Children.Add(border);
                grid_MainGrid.Children.Add(new Line
                {
                    X1 = offsetPx,
                    X2 = offsetPx,
                    Y1 = 0,
                    Y2 = grid_MainGrid.ActualHeight,
                    Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00)),
                    StrokeThickness = 1,
                });
                subTicks.ForEach(x => grid_MainGrid.Children.Add(new Line
                {
                    
                    X1 = offsetPx + x.Margin.Left + x.Width / 2,
                    X2 = offsetPx + x.Margin.Left + x.Width / 2,
                    Y1 = 0,
                    Y2 = grid_MainGrid.ActualHeight,
                    Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x88, 0x00)),
                    StrokeThickness = 1,
                }));
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
