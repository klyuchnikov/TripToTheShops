﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml.Linq;

namespace TripToTheShops
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                if (File.Exists(e.Args[0]))
                {
                    var doc = XDocument.Load(e.Args[0]);
                    Model.Current.LoadShops(doc);
                }

            }
        }
    }
}
