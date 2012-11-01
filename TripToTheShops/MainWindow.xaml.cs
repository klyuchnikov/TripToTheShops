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

        private void PaintShopsToCanvas()
        {
            canvas1.Width = 0;
            canvas1.Height = 0;
            canvas1.Children.Clear();
            if (Model.Current.Shops.Length == 0)
                return;
            var minX = Model.Current.Shops.Select(q => q.Coordinates.X).Min();
            var minY = Model.Current.Shops.Select(q => q.Coordinates.Y).Min();
            var min = minX < minY ? minX : minY;
            min = Math.Abs(min) + 100;
            foreach (var shop in Model.Current.Shops)
            {
                Image image = new Image
                {
                    Width = 48,
                    Source = new BitmapImage(new Uri(@"/TripToTheShops;component/magaz.png", UriKind.Relative))
                };
                Canvas.SetLeft(image, shop.Coordinates.X + min);
                Canvas.SetTop(image, shop.Coordinates.Y + min);
                canvas1.Children.Add(image);
                Label labelName = new Label() { Content = shop.Name };
                Canvas.SetLeft(labelName, shop.Coordinates.X + min + 50);
                Canvas.SetTop(labelName, shop.Coordinates.Y + min);
                canvas1.Children.Add(labelName);
                if (shop.Coordinates.X + min + 100 > canvas1.Width)
                    canvas1.Width = shop.Coordinates.X + min + 100;
                if (shop.Coordinates.Y + min + 100 > canvas1.Height)
                    canvas1.Height = shop.Coordinates.Y + min + 100;
            }
            // set icon home
            Image imageHome = new Image
            {
                Width = 48,
                Source = new BitmapImage(new Uri(@"/TripToTheShops;component/kfm_home.png", UriKind.Relative))
            };
            Canvas.SetLeft(imageHome, min);
            Canvas.SetTop(imageHome, min);
            canvas1.Children.Add(imageHome);
            canvas1.UpdateLayout();
        }
    }
}
