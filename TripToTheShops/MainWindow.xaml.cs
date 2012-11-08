﻿using System;
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
using System.Globalization;

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
            if (RBminCost.IsChecked == true)
                PlanMinimizeCost(selectedProducts);
            if (RBminDist.IsChecked == true)
                PlanMinimizeDist(selectedProducts);
        }

        private void PlanMinimizeDist(Product[] selectedProducts)
        {
            var orderToDistShopsToHome = Model.Current.Shops.OrderBy(q => GetDistance(new Point(0, 0), q.Coordinates)).ToList();
            var remainingProducts = new List<Product>(selectedProducts);
            var totalDist = 0.0;
            var totalCost = 0.0;
            var listProducts = new Dictionary<string, List<Product>>();
            do
            {
                Shop shop = orderToDistShopsToHome.First(a => a.Products.Intersect(remainingProducts, new ProductComparer()).Count() != 0);
                totalDist += GetDistance(new Point(0, 0), shop.Coordinates);
                listProducts.Add(shop.ID, new List<Product>());
                foreach (var a in shop.Products)
                {
                    if (remainingProducts.Contains(a, new ProductComparer()))
                    {
                        listProducts[shop.ID].Add(a);
                        remainingProducts.Remove(remainingProducts.Single(q => q.Code == a.Code));
                        totalCost += a.Price;
                    }
                }
                orderToDistShopsToHome.Remove(shop);
            } while (remainingProducts.Count != 0);
            totalDist += GetDistance(Model.Current.Shops.Single(a => a.ID == listProducts.Last().Key).Coordinates, new Point(0, 0));


        }


        private void PlanMinimizeCost(Product[] selectedProducts)
        {
            var listProducts = new Dictionary<string, List<Product>>();
            var totalDist = 0.0;
            var totalCost = 0.0;
            foreach (var p in selectedProducts)
            {
                var prod = Model.Current.Shops.SelectMany(q => q.Products).Where(a => a.Code == p.Code).OrderBy(q => q.Price).First();
                if (listProducts.ContainsKey(prod.Shop.ID))
                    listProducts[prod.Shop.ID].Add(prod);
                else
                    listProducts.Add(prod.Shop.ID, new List<Product>(new[] { prod }));
            }
            var first = Model.Current.Shops.Single(a => a.ID == listProducts.First().Key);
            PaintConnectShops(min, new Point(0, 0), first.Coordinates);
            totalDist += GetDistance(new Point(0, 0), first.Coordinates);
            totalCost += listProducts.First().Value.Sum(q => q.Price);

            for (int i = 0; i < listProducts.Keys.Count - 1; i++)
            {
                var shopSource = Model.Current.Shops.Single(a => a.ID == listProducts.Keys.ElementAt(i));
                var shopDest = Model.Current.Shops.Single(a => a.ID == listProducts.Keys.ElementAt(i + 1));
                PaintConnectShops(min, shopSource.Coordinates, shopDest.Coordinates);
                PaintLabels(listProducts[listProducts.Keys.ElementAt(i)], shopSource.Coordinates, shopDest.Coordinates, shopSource.Name);
                totalDist += GetDistance(shopSource.Coordinates, shopDest.Coordinates);
                totalCost += listProducts[listProducts.Keys.ElementAt(i + 1)].Sum(q => q.Price);
            }
            var last = Model.Current.Shops.Single(a => a.ID == listProducts.Last().Key);
            PaintLabels(listProducts[listProducts.Keys.Last()], last.Coordinates, new Point(0, 0), last.Name);
            PaintConnectShops(min, last.Coordinates, new Point(0, 0));
            totalDist += GetDistance(last.Coordinates, new Point(0, 0));
            labelCost.Content = string.Format("{0:G7}", totalCost);
            labelDistance.Content = totalDist;

            var otherShops = Model.Current.Shops.Where(q => !listProducts.ContainsKey(q.ID)).ToArray();
            foreach (var shop in otherShops)
            {
                canvas1.Children.Add(new Label() { Content = shop.Name, Margin = new Thickness(min + shop.Coordinates.X + 50, min + shop.Coordinates.Y, 0, 0) });
            }
        }

        private double GetDistance(Point s, Point d)
        {
            return Math.Abs(s.X - d.X) + Math.Abs(s.Y - d.Y);
        }

        private void PaintLabels(List<Product> listProducts, Point cs, Point cd, string str)
        {
            var grid = new Grid() { Width = 500, Height = 500 };
            canvas1.Children.Add(grid);
            var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch), this.FontSize, Brushes.Black);

            if (cs.X > cd.X)
                grid.Margin = new Thickness(cs.X + min + 50, cs.Y + min, 0, 0);
            else
                grid.Margin = new Thickness(cs.X + min - formattedText.Width - 10, cs.Y + min, 0, 0);

            grid.Children.Add(new Line() { X2 = formattedText.Width, Stroke = Brushes.Black, StrokeThickness = 2, Margin = new Thickness(6, 25, 0, 0) });
            var labelname = new Label() { Content = str };

            grid.Children.Add(labelname);
            for (var k = 0; k < listProducts.Count; k++)
            {
                var p = listProducts[k];
                grid.Children.Add(new Label() { Content = p.Name, Margin = new Thickness(0, 25 + k * 15, 0, 0) });
            }
        }
    }
}
