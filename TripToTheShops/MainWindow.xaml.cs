using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace TripToTheShops
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            globalGrid.DataContext = Model.Current;
            PaintShopsToCanvas();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "XML Files|*.xml";
            OFD.ShowDialog();
            string FileName = OFD.FileName;
            globalGrid.DataContext = null;
            Model.Current.LoadShops(OFD.FileName);
            globalGrid.DataContext = Model.Current;
            globalGrid.UpdateLayout();
            PaintShopsToCanvas();
        }

        private void PaintConnectShops(double min, Point source, Point destination)
        {
            //  var shopSource = source.Tag as Shop;
            //   var shopDestinaion = destination.Tag as Shop;
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
            var wide = 16;
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
            // Canvas.SetLeft(polygon, min + sd.X + 24 - (wide / 2));
            // Canvas.SetTop(polygon, min + sd.Y - 8);
            canvas1.UpdateLayout();
        }

        private double min = 0;

        private void PaintShopsToCanvas()
        {
            canvas1.Width = 0;
            canvas1.Height = 0;
            canvas1.Children.Clear();
            if (Model.Current.Shops.Length == 0)
                return;
            var minX = Model.Current.Shops.Select(q => q.Coordinates.X).Min();
            var minY = Model.Current.Shops.Select(q => q.Coordinates.Y).Min();
            min = minX < minY ? minX : minY;
            min = Math.Abs(min) + 100;
            var imgs = new List<Image>();

            foreach (var shop in Model.Current.Shops)
            {
                Image image = new Image
                {
                    Width = 48,
                    Tag = shop.Coordinates,
                    Source = new BitmapImage(new Uri(@"/TripToTheShops;component/magaz.png", UriKind.Relative))
                };
                Canvas.SetLeft(image, shop.Coordinates.X + min);
                Canvas.SetTop(image, shop.Coordinates.Y + min);
                canvas1.Children.Add(image);
                imgs.Add(image);

                if (shop.Coordinates.X + min + 100 > canvas1.Width)
                    canvas1.Width = shop.Coordinates.X + min + 100;
                if (shop.Coordinates.Y + min + 100 > canvas1.Height)
                    canvas1.Height = shop.Coordinates.Y + min + 100;
            }
            // set icon home
            Image imageHome = new Image
            {
                Width = 48,
                Tag = new Point(0, 0),
                Source = new BitmapImage(new Uri(@"/TripToTheShops;component/kfm_home.png", UriKind.Relative))
            };
            Canvas.SetLeft(imageHome, min);
            Canvas.SetTop(imageHome, min);

            //     PaintConnectShops(min, (Point)imgs[0].Tag, (Point)imgs[1].Tag);
            canvas1.Children.Add(imageHome);
            canvas1.UpdateLayout();
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
            var listProducts = new Dictionary<string, List<Product>>();
            foreach (var p in selectedProducts)
            {
                var prod = Model.Current.Shops.SelectMany(q => q.Products).Where(a => a.Code == p.Code).OrderBy(q => q.Price).First();
                if (listProducts.ContainsKey(prod.Shop.ID))
                    listProducts[prod.Shop.ID].Add(prod);
                else
                    listProducts.Add(prod.Shop.ID, new List<Product>(new[] { prod }));
            }

            PaintConnectShops(min, new Point(0, 0), Model.Current.Shops.Single(a => a.ID == listProducts.First().Key).Coordinates);
            for (int i = 0; i < listProducts.Keys.Count - 1; i++)
            {
                var shopSource = Model.Current.Shops.Single(a => a.ID == listProducts.Keys.ElementAt(i));
                var shopDest = Model.Current.Shops.Single(a => a.ID == listProducts.Keys.ElementAt(i + 1));
                PaintConnectShops(min, shopSource.Coordinates, shopDest.Coordinates);
                var grid = new Grid() { Width = 200, Height = 500 };
                canvas1.Children.Add(grid);
                if (shopSource.Coordinates.X > shopDest.Coordinates.X)
                    grid.Margin = new Thickness(shopSource.Coordinates.X + min + 50, shopSource.Coordinates.Y + min, 0, 0);
                else
                    grid.Margin = new Thickness(shopSource.Coordinates.X + min - 100, shopSource.Coordinates.Y + min, 0, 0);

                grid.Children.Add(new Line() { X2 = 50, Stroke = Brushes.Black, StrokeThickness = 2, Margin = new Thickness(6, 25, 0, 0) });
                grid.Children.Add(new Label() { Content = shopSource.Name });
                for (var k = 0; k < listProducts[listProducts.Keys.ElementAt(i)].Count; k++)
                {
                    
                }
            }

            PaintConnectShops(min, Model.Current.Shops.Single(a => a.ID == listProducts.Last().Key).Coordinates, new Point(0, 0));
        }
    }
}
