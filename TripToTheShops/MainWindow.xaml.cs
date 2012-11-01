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
            var el = new Ellipse() { Height = 30, Width = 30 };
            el.Fill = Brushes.Red;
            //     Canvas.SetLeft(el, 0);
            //     Canvas.SetTop(el, 0);
            //    canvas1.Children.Add(el);

            canvas1.UpdateLayout();
            Image image = new Image
            {
                Width = 48,
                Source = new BitmapImage(new Uri(@"/TripToTheShops;component/magaz.png", UriKind.Relative))//,
               // Margin = new Thickness(100,0,0,0)
            };
            Canvas.SetLeft(image, 500);
            canvas1.Children.Add(image);
            canvas1.Width = 500 + 200;
            canvas1.UpdateLayout();
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

        }
    }
}
