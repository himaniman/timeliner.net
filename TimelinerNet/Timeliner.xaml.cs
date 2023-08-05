﻿using System;
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
        private DateTime initCaptureLeftEdge;
        private DateTime initCaptureRightEdge;
        private TimeSpan initCaptureScalePx;

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
                initCaptureLeftEdge = LeftEdge;
                initCaptureRightEdge = RightEdge;
                initCaptureScalePx = (RightEdge - LeftEdge) / grid_Timeline.ActualWidth;
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

                
                //var span = RightEdge - LeftEdge;
                LeftEdge = initCaptureLeftEdge + deltapx * initCaptureScalePx;
                RightEdge = initCaptureRightEdge + deltapx * initCaptureScalePx;
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
                    LeftEdge -= span * 0.05;
                    RightEdge += span * 0.05;
                }
                else
                {
                    LeftEdge += span * 0.05;
                    RightEdge -= span * 0.05;
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
                        Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x88, 0x00)),
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
                    Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00)),
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
            //var timePerPixcel = span / xSize;

            foreach (var item in Items)
            {
                stackPanel_Threads.Children.Add(new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00)),
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Child = new TextBlock { Text = item.Value.Name, Margin = new Thickness(2) },
                });
                var bd = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00)),
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Child = new Grid()
                };
                foreach (var job in item.Value.Jobs)
                {
                    if (job.Value.End > LeftEdge && job.Value.Begin < RightEdge)
                    {
                        var br = new Border
                        {
                            Width = (job.Value.End - job.Value.Begin).ToPixcel(span, xSize),
                            Margin = new Thickness((job.Value.Begin - LeftEdge).ToPixcel(span, xSize), 0, 0, 0),
                            CornerRadius = new CornerRadius(4),
                            Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF)),
                            BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF)),
                            BorderThickness = new Thickness(1),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,
                            Child = new TextBlock
                            {
                                Text = job.Value.Name,
                                Margin = new Thickness(1)
                            }
                        };
                        if (br.Margin.Left < 0)
                        {
                            br.Width = br.Width + br.Margin.Left;
                            br.Margin = new Thickness(0);
                            (br.Child as TextBlock).Text = "<-" + (br.Child as TextBlock).Text;
                        }
                        if (br.Margin.Left + br.Width > xSize)
                        {
                            //br.Width = br.Margin.Left - br.Width;
                            (br.Child as TextBlock).Text = (br.Child as TextBlock).Text + "->";
                        }
                    (bd.Child as Grid).Children.Add(br);
                    }
                }

                stackPanel_MainData.Children.Add(bd);
            }
        }
    }
}
