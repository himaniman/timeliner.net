using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        public event PropertyChangedEventHandler PropertyChanged;
        private DateTime Now;
        private DateTime LeftEdge;
        private DateTime RightEdge;
        private Point initMousePoint;
        private DateTime initCaptureLeftEdge;
        private DateTime initCaptureRightEdge;
        private TimeSpan initCaptureScalePx;

        public bool IsOnManipulate { get; set; }
        public bool? TestTEst { get; set; }

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



        public DataTemplate DataTemplatePopup
        {
            get { return (DataTemplate)GetValue(DataTemplatePopupProperty); }
            set { SetValue(DataTemplatePopupProperty, value); }
        }

        public static readonly DependencyProperty DataTemplatePopupProperty =
            DependencyProperty.Register("DataTemplatePopup", typeof(DataTemplate), typeof(Timeliner), new PropertyMetadata(null));



        public Timeliner()
        {
            Now = new DateTime(2023, 08, 20, 23, 50, 00);
            LeftEdge = Now - TimeSpan.FromMinutes(60);
            RightEdge = Now + TimeSpan.FromMinutes(30);
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
                var span = RightEdge - LeftEdge;
                if (e.Delta < 0)
                {
                    LeftEdge -= span * 0.05;
                    RightEdge += span * 0.05;
                }
                else
                {
                    LeftEdge += span * 0.05;
                    RightEdge -= span * 0.05;
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

        public void RedrawGrid()
        {
            var xSize = grid_Timeline.ActualWidth;
            grid_Timeline.Children.Clear();
            grid_MainGrid.Children.Clear();

            var span = RightEdge - LeftEdge;
            var majorMode = span.NearSpanMode();
            var minorMode = span.NearSpanMode() - 1;
            //var timePerPixcel = span / xSize;

            var leftEdgeMajor = LeftEdge.GetMajorLeftEdge(majorMode);
            //var currentMajorTurn = 0;
            var currentMajor = leftEdgeMajor;

            while (currentMajor <= RightEdge)
            {
                var majorSpan = majorMode.ModeToSpan(currentMajor);
                var majorWithPx = majorSpan.ToPixcel(span, xSize);
                //currentMajor = leftEdgeMajor + major * currentMajorTurn;
                var majorOffset = currentMajor - leftEdgeMajor - (LeftEdge - leftEdgeMajor);
                var majorOffsetPx = majorOffset.ToPixcel(span, xSize);

                var border = new Border
                {
                    Width = majorWithPx,
                    Margin = new Thickness(majorOffsetPx, 0, 0, 0),
                    //Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xAC, 0xAC, 0xAC)),
                    BorderBrush = SystemColors.ActiveBorderBrush,
                    BorderThickness = new Thickness(1, 0, 1, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Child = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = currentMajor.ToFullString(majorMode),
                                HorizontalAlignment = majorOffsetPx < 0 ? HorizontalAlignment.Right : majorOffsetPx + majorWithPx > xSize ? HorizontalAlignment.Left : HorizontalAlignment.Center,
                                Margin = new Thickness(2)
                            },
                            new Grid
                            {
                                ClipToBounds = true,
                            }
                        }
                    }
                };

                var currentMinor = currentMajor;
                var minorSpan = minorMode.ModeToSpan(currentMinor) * 15;
                while (currentMinor < currentMajor + majorSpan - minorSpan)
                {
                    currentMinor += minorSpan;
                    minorSpan = minorMode.ModeToSpan(currentMinor) * 15;
                    var minorWithPx = minorSpan.ToPixcel(span, xSize);
                    var minorOffset = currentMinor - currentMajor;
                    var minorOffsetPx = minorOffset.ToPixcel(span, xSize);
                    ((border.Child as StackPanel).Children[1] as Grid).Children.Add(new TextBlock
                    {
                        Text = currentMinor.ToLastString(minorMode),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        TextAlignment = TextAlignment.Center,
                        Width = minorWithPx,
                        Margin = new Thickness(minorOffsetPx - minorWithPx / 2, 2, 2, 2)
                    });
                    grid_MainGrid.Children.Add(new Line
                    {
                        X1 = majorOffsetPx + minorOffsetPx,
                        X2 = majorOffsetPx + minorOffsetPx,
                        Y1 = 0,
                        Y2 = grid_MainGrid.ActualHeight,
                        Stroke = SystemColors.ActiveBorderBrush,
                        StrokeThickness = 1,
                    });
                }

                grid_Timeline.Children.Add(border);
                grid_MainGrid.Children.Add(new Line
                {
                    X1 = majorOffsetPx,
                    X2 = majorOffsetPx,
                    Y1 = 0,
                    Y2 = grid_MainGrid.ActualHeight,
                    Stroke = SystemColors.ActiveBorderBrush,
                    StrokeThickness = 1,
                });
                currentMajor += majorSpan;
            }
            RedrawData();
        }

        public void RedrawData()
        {
            var xSize = grid_Timeline.ActualWidth;
            stackPanel_Threads.Children.Clear();
            stackPanel_MainData.Children.Clear();

            var span = RightEdge - LeftEdge;
            
            TextBlock test = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Text = "Test",
                FontSize = FontSize
            };
            test.Measure(new Size(100, 100));
            double heightText = test.DesiredSize.Height;

            foreach (var item in Items)
            {
                stackPanel_Threads.Children.Add(new Border
                {
                    BorderBrush = SystemColors.ActiveBorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Child = new TextBlock { Text = item.Value.Name, Margin = new Thickness(2) },
                    Height = heightText + 4,
                });
                var bd = new Border
                {
                    BorderBrush = SystemColors.ActiveBorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Child = new Grid(),
                    Height = heightText + 4,
                };
                double lastItemLeftEdge = double.NaN;
                foreach (var job in item.Value.Jobs.OrderByDescending(x => x.Value.Begin))
                {
                    if (job.Value.End > LeftEdge && job.Value.Begin < RightEdge)
                    {
                        double width = (job.Value.End - job.Value.Begin).ToPixcel(span, xSize);
                        var gr = new Grid
                        {
                            Margin = new Thickness((job.Value.Begin - LeftEdge).ToPixcel(span, xSize), 0, 0, 0),
                            Background = Brushes.Transparent,
                            HorizontalAlignment = HorizontalAlignment.Left,
                        };
                        gr.PreviewMouseDown += (s, e) =>
                        {
                            //if ((s as Grid).Children[0] is Border)
                            //{
                            //    ((s as Grid).Children[0] as Border).Background = job.Value.Color.Clone();
                            //}
                            cc_info.ContentTemplate = DataTemplatePopup ?? (DataTemplate)this.Resources["defaultTemplate"];
                            cc_info.Content = job.Value;
                            popup_info.IsOpen = true;
                            Mouse.Capture(this);
                            e.Handled = true;
                        };

                        if (width > 6)
                        {
                            var br = new Border
                            {
                                Width = (job.Value.End - job.Value.Begin).ToPixcel(span, xSize),
                                CornerRadius = new CornerRadius(4),
                                Background = !job.Value.IsStripedColor ? job.Value.Color.Clone() 
                                    : new LinearGradientBrush
                                    {
                                        MappingMode = BrushMappingMode.Absolute,
                                        EndPoint = new Point(8, 8),
                                        SpreadMethod = GradientSpreadMethod.Repeat,
                                        GradientStops =
                                        {
                                            new GradientStop
                                            {
                                                Offset = 0,
                                                Color = ((SolidColorBrush)job.Value.Color).Color
                                            },
                                            new GradientStop
                                            {
                                                Offset = 0.5,
                                                Color = ((SolidColorBrush)job.Value.Color).Color
                                            },
                                            new GradientStop
                                            {
                                                Offset = 0.5,
                                                Color = Colors.LightGray
                                            },
                                            new GradientStop
                                            {
                                                Offset = 1,
                                                Color = Colors.LightGray
                                            }
                                        }
                                    },
                                BorderBrush = SystemColors.ActiveBorderBrush,
                                BorderThickness = new Thickness(1),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                                Child = new TextBlock
                                {
                                    Text = job.Value.Name,
                                    Margin = new Thickness(1)
                                }
                            };
                            if (gr.Margin.Left < 0)
                            {
                                br.Width = br.Width + gr.Margin.Left;
                                gr.Margin = new Thickness(0);
                                (br.Child as TextBlock).Text = "<-" + (br.Child as TextBlock).Text;
                            }
                            if (gr.Margin.Left + br.Width > xSize)
                            {
                                //br.Width = br.Margin.Left - br.Width;
                                (br.Child as TextBlock).Text = (br.Child as TextBlock).Text + "->";
                            }
                            lastItemLeftEdge = (job.Value.Begin - LeftEdge).ToPixcel(span, xSize);
                            gr.Children.Add(br);
                        }
                        else
                        {
                            var ln = new Path
                            {
                                Data = Geometry.Parse($"M 0 {heightText} L3 {heightText - 3} L6 {heightText} L3 {heightText - 3} L3 3 L6 0 L3 3 L0 0 L3 3 L3 {heightText - 3} Z"),
                                Width = 6,
                                Height = heightText,
                                Stroke = job.Value.Color.Clone(),
                                StrokeThickness = 2,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                            };
                            test = new TextBlock
                            {
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                Text = job.Value.Name,
                                FontSize = FontSize
                            };
                            gr.Children.Add(ln);

                            test.Measure(new Size(100, 100));
                            var oversize = (job.Value.Begin - LeftEdge).ToPixcel(span, xSize) + test.DesiredSize.Width + 8 > lastItemLeftEdge;
                            if (!oversize)
                            {
                                var tx = new TextBlock
                                {
                                    Text = job.Value.Name,
                                    Margin = new Thickness(6, 2, 2, 2)
                                };
                                gr.Children.Add(tx);
                            }
                            lastItemLeftEdge = (job.Value.Begin - LeftEdge).ToPixcel(span, xSize);
                        }
                    (bd.Child as Grid).Children.Add(gr);
                    }
                }

                stackPanel_MainData.Children.Add(bd);
            }
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
