using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Globalization;

namespace TripToTheShops
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _currentCoordinatesHome = new Point(0, 0);
        /// <summary>
        /// Конструктор
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            globalGrid.DataContext = Model.Current;
            PaintShopsToCanvas();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "XML Files|*.xml" };
            ofd.ShowDialog();
            globalGrid.DataContext = null;
            Model.Current.LoadShops(ofd.FileName);
            globalGrid.DataContext = Model.Current;
            globalGrid.UpdateLayout();
            PaintShopsToCanvas();
        }
        /// <summary>
        /// Рисование соединения между двумя точками на Canvas 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private void PaintConnectShops(double min, Point source, Point destination)
        {
            var ss = source;
            var sd = destination;
            var polyline = new Polyline()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3,
            };
            var polygon = new Polygon()
            {
                Fill = Brushes.Black,
                StrokeThickness = 3,
            };
            const int wide = 16;
            if (ss.X > sd.X)
                if (ss.Y > sd.Y)
                {
                    polyline.Points.Add(new Point(ss.X + 2, ss.Y + 24));
                    polyline.Points.Add(new Point(sd.X + 24, ss.Y + 24));
                    polyline.Points.Add(new Point(sd.X + 24, sd.Y + 48));
                    polygon.Points.Add(new Point(wide / 2, 10 * 1.1));
                    polygon.Points.Add(new Point(wide, wide * 1.1));
                    polygon.Points.Add(new Point(0, wide * 1.1));
                    Canvas.SetLeft(polygon, min + sd.X + 24 - (wide / 2));
                    Canvas.SetTop(polygon, min + sd.Y + 34);
                }
                else
                {
                    polyline.Points.Add(new Point(ss.X + 2, ss.Y + 24));
                    polyline.Points.Add(new Point(sd.X + 24, ss.Y + 24));
                    polyline.Points.Add(new Point(sd.X + 24, sd.Y));
                    polygon.Points.Add(new Point(0, 0));
                    polygon.Points.Add(new Point(wide, 0));
                    polygon.Points.Add(new Point(wide / 2, 10));
                    Canvas.SetLeft(polygon, min + sd.X + 24 - (wide / 2));
                    Canvas.SetTop(polygon, min + sd.Y - 8);
                }
            else
                if (ss.Y > sd.Y)
                {
                    polyline.Points.Add(new Point(ss.X + 45, ss.Y + 24));
                    polyline.Points.Add(new Point(sd.X + 24, ss.Y + 24));
                    polyline.Points.Add(new Point(sd.X + 24, sd.Y + 45));

                    polygon.Points.Add(new Point(wide / 2, 10 * 1.1));
                    polygon.Points.Add(new Point(wide, wide * 1.1));
                    polygon.Points.Add(new Point(0, wide * 1.1));
                    Canvas.SetLeft(polygon, min + sd.X + 24 - (wide / 2));
                    Canvas.SetTop(polygon, min + sd.Y + 34);
                }
                else
                {
                    polyline.Points.Add(new Point(ss.X + 45, ss.Y + 24));
                    polyline.Points.Add(new Point(sd.X + 24, ss.Y + 24));
                    polyline.Points.Add(new Point(sd.X + 24, sd.Y));
                    polygon.Points.Add(new Point(0, 0));
                    polygon.Points.Add(new Point(wide, 0));
                    polygon.Points.Add(new Point(wide / 2, 10));
                    Canvas.SetLeft(polygon, min + sd.X + 24 - (wide / 2));
                    Canvas.SetTop(polygon, min + sd.Y - 8);
                }

            canvas1.Children.Add(polyline);
            Canvas.SetLeft(polyline, min);
            Canvas.SetTop(polyline, min);

            canvas1.Children.Add(polygon);
            canvas1.UpdateLayout();
        }

        private double _min = 0;

        /// <summary>
        /// Рисование магазинов и дома
        /// </summary>
        private void PaintShopsToCanvas()
        {
            canvas1.Width = 0;
            canvas1.Height = 0;
            canvas1.Children.Clear();
            if (Model.Current.Shops.Length == 0)
                return;
            var minX = Model.Current.Shops.Select(q => q.Coordinates.X).Min();
            var minY = Model.Current.Shops.Select(q => q.Coordinates.Y).Min();
            _min = minX < minY ? minX : minY;
            _min = Math.Abs(_min) + 100;
            var imgs = new List<Image>();

            foreach (var shop in Model.Current.Shops)
            {
                var image = new Image
                {
                    Width = 48,
                    Tag = shop.Coordinates,
                    Source = new BitmapImage(new Uri(@"/TripToTheShops;component/magaz.png", UriKind.Relative))
                };
                Canvas.SetLeft(image, shop.Coordinates.X + _min);
                Canvas.SetTop(image, shop.Coordinates.Y + _min);
                canvas1.Children.Add(image);
                imgs.Add(image);

                if (shop.Coordinates.X + _min + 100 > canvas1.Width)
                    canvas1.Width = shop.Coordinates.X + _min + 100;
                if (shop.Coordinates.Y + _min + 100 > canvas1.Height)
                    canvas1.Height = shop.Coordinates.Y + _min + 100;
            }
            // set icon home
            var imageHome = new Image
            {
                Width = 48,
                Tag = _currentCoordinatesHome,
                Source = new BitmapImage(new Uri(@"/TripToTheShops;component/kfm_home.png", UriKind.Relative))
            };
            Canvas.SetLeft(imageHome, _min + _currentCoordinatesHome.X);
            Canvas.SetTop(imageHome, _min + _currentCoordinatesHome.Y);
            imageHome.Cursor = Cursors.ScrollAll;
            Canvas.SetZIndex(imageHome, 1000);
            imageHome.MouseMove += new MouseEventHandler(imageHome_MouseMove);
            imageHome.MouseDown += new MouseButtonEventHandler(imageHome_MouseDown);
            imageHome.MouseUp += new MouseButtonEventHandler(imageHome_MouseUp);
            imageHome.UpdateLayout();
            canvas1.Children.Add(imageHome);
            PrintCoordinatesHome(imageHome);
            canvas1.UpdateLayout();
        }

        private Point lastPoint;
        void imageHome_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        void imageHome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastPoint = Mouse.GetPosition(canvas1);
        }

        void imageHome_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPoint = Mouse.GetPosition(canvas1);
                var dp = currentPoint - lastPoint;
                var image = sender as Image;
                Canvas.SetLeft(image, Canvas.GetLeft(image) + dp.X);
                Canvas.SetTop(image, Canvas.GetTop(image) + dp.Y);
                canvas1.UpdateLayout();
                image.Tag = _currentCoordinatesHome + dp;
                _currentCoordinatesHome = _currentCoordinatesHome + dp;
                lastPoint = Mouse.GetPosition(canvas1);
                PrintCoordinatesHome(image);
                button2_Click(null, e);
            }
        }


        private void button2_Click(object sender, RoutedEventArgs e)
        {
            PaintShopsToCanvas();
            var selectedProducts = listShops.SelectedItems.OfType<Product>().ToArray();
            if (selectedProducts.Length == 0)
            {
                MessageBox.Show("Select products!");
                return;
            }
            if (RBminCost.IsChecked == RBminDist.IsChecked)
            {
                MessageBox.Show("Select goal!");
                return;
            }
            if (RBminCost.IsChecked == true)
                PaintPlan(Model.Current.PlanMinimizeCost(selectedProducts));
            if (RBminDist.IsChecked == true)
                PaintPlan(Model.Current.PlanMinimizeDist(selectedProducts, _currentCoordinatesHome));
        }

        /// <summary>
        /// Рисование плана покупок
        /// </summary>
        /// <param name="listProducts"></param>
        private void PaintPlan(Dictionary<string, List<Product>> listProducts)
        {
            var totalDist = 0.0;
            var totalCost = 0.0;
            var first = Model.Current.Shops.Single(a => a.ID == listProducts.First().Key);
            PaintConnectShops(_min, _currentCoordinatesHome, first.Coordinates);
            totalDist += Model.Current.GetDistance(_currentCoordinatesHome, first.Coordinates);
            totalCost += listProducts.First().Value.Sum(q => q.Price);

            for (int i = 0; i < listProducts.Keys.Count - 1; i++)
            {
                var shopSource = Model.Current.Shops.Single(a => a.ID == listProducts.Keys.ElementAt(i));
                var shopDest = Model.Current.Shops.Single(a => a.ID == listProducts.Keys.ElementAt(i + 1));
                PaintConnectShops(_min, shopSource.Coordinates, shopDest.Coordinates);
                PaintLabels(listProducts[listProducts.Keys.ElementAt(i)], shopSource.Coordinates, shopSource.Coordinates.X > shopDest.Coordinates.X, shopSource.Name);
                totalDist += Model.Current.GetDistance(shopSource.Coordinates, shopDest.Coordinates);
                totalCost += listProducts[listProducts.Keys.ElementAt(i + 1)].Sum(q => q.Price);
            }
            var last = Model.Current.Shops.Single(a => a.ID == listProducts.Last().Key);
            PaintLabels(listProducts[listProducts.Keys.Last()], last.Coordinates, last.Coordinates.X > 0, last.Name);
            PaintConnectShops(_min, last.Coordinates, _currentCoordinatesHome);
            totalDist += Model.Current.GetDistance(last.Coordinates, _currentCoordinatesHome);
            labelCost.Content = string.Format("{0:G7}", totalCost);
            labelDistance.Content = totalDist;

            var otherShops = Model.Current.Shops.Where(q => !listProducts.ContainsKey(q.ID)).ToArray();
            foreach (var shop in otherShops)
            {
                canvas1.Children.Add(new Label() { Content = shop.Name, Margin = new Thickness(_min + shop.Coordinates.X + 50, _min + shop.Coordinates.Y, 0, 0) });
            }
        }

        /// <summary>
        /// Рисование названий продуктов около магазина
        /// </summary>
        /// <param name="listProducts">список продуктов</param>
        /// <param name="cs">точка магазина</param>
        /// <param name="isRight">Расположение названий относительно магазина</param>
        /// <param name="str">имя магазина</param>
        private void PaintLabels(List<Product> listProducts, Point cs, bool isRight, string str)
        {
            var grid = new Grid() { Width = 500, Height = 500 };
            canvas1.Children.Add(grid);
            var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch), this.FontSize, Brushes.Black);

            if (isRight)
                grid.Margin = new Thickness(cs.X + _min + 50, cs.Y + _min, 0, 0);
            else
                grid.Margin = new Thickness(cs.X + _min - formattedText.Width - 10, cs.Y + _min, 0, 0);

            grid.Children.Add(new Line() { X2 = formattedText.Width, Stroke = Brushes.Black, StrokeThickness = 2, Margin = new Thickness(6, 25, 0, 0) });
            var labelname = new Label() { Content = str };

            grid.Children.Add(labelname);
            for (var k = 0; k < listProducts.Count; k++)
            {
                var p = listProducts[k];
                grid.Children.Add(new Label() { Content = p.Name, Margin = new Thickness(0, 25 + k * 15, 0, 0) });
            }
        }

        private void PrintCoordinatesHome(Image imageHome)
        {
            var point = _currentCoordinatesHome;
            var label = canvas1.Children.OfType<Label>().SingleOrDefault(q => q.Name == "coord_home");
            if (label == null)
            {
                label = new Label { Name = "coord_home", Content = string.Format("({0}, {1})", point.X, point.Y) };
                canvas1.Children.Add(label);
            }
            else
            {
                label.Content = string.Format("({0}, {1})", point.X, point.Y);
            }
            label.Margin = new Thickness(Canvas.GetLeft(imageHome) + 50, Canvas.GetTop(imageHome), 0, 0);
            label.UpdateLayout();
            canvas1.UpdateLayout();
        }
    }
}
