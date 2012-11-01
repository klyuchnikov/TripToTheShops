using System;
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
                    if (Model.Current.LoadShops(e.Args[0]))
                        Model.Current.AddLog("Load shops is successful.");
                    else
                        Model.Current.AddLog("Load shops is failed.");
                }
                else
                     Model.Current.AddLog("File is not exists.");
                if (e.Args.Length > 1)
                    if (File.Exists(e.Args[1]))
                    {
                        if (Model.Current.LoadShoppingList(e.Args[1]))
                            Model.Current.AddLog("Load shoppingList is successful.");
                        else
                            Model.Current.AddLog("Load shoppingList is failed.");
                    }
                    else
                        Model.Current.AddLog("File is not exists.");
                else
                    Model.Current.AddLog("Parameter 'shoppingList' is not specified.");
            }
        }
    }
}
