using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace TimelinerNet
{
    public partial class Timeliner
    {
        public void RedrawGrid()
        {
            if (grid_Timeline == null) return;
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
            var nowOffset = (Now - LeftEdge).ToPixcel(span, xSize);
            grid_Timeline.Children.Add(new Line
            {
                X1 = nowOffset,
                X2 = nowOffset,
                Y1 = 0,
                Y2 = grid_Timeline.ActualHeight,
                Stroke = Brushes.Red,
                StrokeThickness = 1,
            });
            grid_MainGrid.Children.Add(new Line
            {
                X1 = nowOffset,
                X2 = nowOffset,
                Y1 = 0,
                Y2 = grid_MainGrid.ActualHeight,
                Stroke = Brushes.Red,
                StrokeThickness = 1,
            });
            RedrawData();
        }

        public void RedrawData()
        {
            if (grid_Timeline == null) return;
            var xSize = grid_Timeline.ActualWidth;
            stackPanel_Threads.Children.Clear();
            stackPanel_MainData.Children.Clear();
            if (Data.Items == null || !(Data.Items?.Count() > 0)) return;

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

            foreach (var item in Data.Items)
            {
                var isTextUp = (item.Jobs?.Any(x => !string.IsNullOrEmpty(x.TextUp)) ?? false);
                var isTextDown = (item.Jobs?.Any(x => !string.IsNullOrEmpty(x.TextDown)) ?? false);
                var heightItem = heightText + 4 + (isTextUp ? heightText + 2 : 0);
                stackPanel_Threads.Children.Add(new Border
                {
                    BorderBrush = SystemColors.ActiveBorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Child = new TextBlock
                    {
                        Text = item.Name,
                        Margin = new Thickness(2),
                        Foreground = Brushes.White,
                    },
                    Height = heightItem,
                });
                var bd = new Border
                {
                    BorderBrush = SystemColors.ActiveBorderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Child = new Grid(),
                    Height = heightItem,
                };
                double lastItemLeftEdge = double.NaN;
                foreach (var job in item.Jobs.OrderByDescending(x => x.Begin))
                {
                    if (job.End > LeftEdge && job.Begin < RightEdge)
                    {
                        double width = (job.End - job.Begin).ToPixcel(span, xSize);
                        var gr = new Grid
                        {
                            Margin = new Thickness((job.Begin - LeftEdge).ToPixcel(span, xSize), 0, 0, 0),
                            Background = Brushes.Transparent,
                            HorizontalAlignment = HorizontalAlignment.Left,
                        };
                        gr.PreviewMouseDown += (s, e) =>
                        {
                            //if ((s as Grid).Children[0] is Border)
                            //{
                            //    ((s as Grid).Children[0] as Border).Background = job.Color.Clone();
                            //}
                            cc_info.ContentTemplate = DataTemplatePopup ?? (DataTemplate)this.Resources["defaultTemplate"];
                            cc_info.Content = job;
                            popup_info.IsOpen = true;
                            Mouse.Capture(this);
                            e.Handled = true;
                        };

                        if (width > 6)
                        {
                            var br = new Border
                            {
                                Width = (job.End - job.Begin).ToPixcel(span, xSize),
                                Margin = new Thickness(0, isTextUp ? heightText : 0, 0, isTextDown ? heightText : 0),
                                CornerRadius = new CornerRadius(4),
                                Background = !job.IsStripedColor ? job.Color.Clone()
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
                                                Color = ((SolidColorBrush)job.Color).Color
                                            },
                                            new GradientStop
                                            {
                                                Offset = 0.5,
                                                Color = ((SolidColorBrush)job.Color).Color
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
                                    Text = job.Name,
                                    Margin = new Thickness(1)
                                }
                            };
                            if (!string.IsNullOrEmpty(job.TextUp))
                            {
                                test = new TextBlock
                                {
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Bottom,
                                    Text = job.TextUp,
                                    FontSize = FontSize
                                };
                                test.Measure(new Size(100, 100));
                                var oversize = (job.Begin - LeftEdge).ToPixcel(span, xSize) + test.DesiredSize.Width + 8 > lastItemLeftEdge;

                                if (!oversize)
                                {
                                    var textUp = new TextBlock
                                    {
                                        Text = job.TextUp,
                                        Margin = new Thickness(0, 0, 0, heightText + (isTextDown ? heightText : 0))
                                    };
                                    gr.Children.Add(textUp);
                                }
                            }

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
                            lastItemLeftEdge = (job.Begin - LeftEdge).ToPixcel(span, xSize);
                            gr.Children.Add(br);
                        }
                        else
                        {
                            var height = heightItem - 4;
                            var ln = new Path
                            {
                                Data = Geometry.Parse(string.Format(cultureInfo, "M 0 {0} L3 {1} L6 {0} L3 {1} L3 3 L6 0 L3 3 L0 0 L3 3 L3 {1} Z", height, height - 3)),
                                Width = 6,
                                Height = height,
                                Stroke = job.Color.Clone(),
                                StrokeThickness = 2,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                            };
                            gr.Children.Add(ln);

                            test = new TextBlock
                            {
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                Text = job.Name,
                                FontSize = FontSize
                            };
                            test.Measure(new Size(100, 100));

                            var oversize = (job.Begin - LeftEdge).ToPixcel(span, xSize) + test.DesiredSize.Width + 8 > lastItemLeftEdge;
                            if (!oversize)
                            {
                                var tx = new TextBlock
                                {
                                    Text = job.Name,
                                    Margin = new Thickness(6, isTextUp ? heightText : 0, 0, isTextDown ? heightText : 0)
                                };
                                gr.Children.Add(tx);
                            }
                            lastItemLeftEdge = (job.Begin - LeftEdge).ToPixcel(span, xSize);
                        }
                    (bd.Child as Grid).Children.Add(gr);
                    }
                }

                stackPanel_MainData.Children.Add(bd);
            }
        }
    }
}
