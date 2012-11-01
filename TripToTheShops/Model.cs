using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows;
using System.IO;

namespace TripToTheShops
{
    public class Model
    {
        /// <summary>
        /// Закрытый конструктор
        /// </summary>
        protected Model()
        {
            this.log = new List<string>();
            startupTime = DateTime.Now.ToString().Replace(':', '_') + ".txt";
        }

        private string startupTime;

        /// <summary>
        /// Внутреннее поле сущности Модели
        /// </summary>
        private static Model current = new Model();

        /// <summary>
        /// Получение текущей модели
        /// </summary>
        public static Model Current { get { return current; } }

        /// <summary>
        /// Список магазинов
        /// </summary>
        public Shop[] Shops { get; private set; }


        /// <summary>
        /// Log (private)
        /// </summary>
        private List<string> log;

        /// <summary>
        /// Log
        /// </summary>
        public string[] Log { get { return log.ToArray(); } }

        /// <summary>
        /// Add text log
        /// </summary>
        /// <param name="str">text</param>
        public void AddLog(string str)
        {
            File.AppendAllText(startupTime, DateTime.Now + " " + str + Environment.NewLine);
            log.Add(str);
        }

        /// <summary>
        /// Загружен ли список магазинов
        /// </summary>
        public bool IsLoadShops { get; private set; }
        /// <summary>
        /// Загрузка магазинов
        /// </summary>
        /// <param name="doc">XML документ</param>
        /// <returns></returns>
        public bool LoadShops(XDocument doc)
        {
            try
            {
                var shopsXML = doc.Root.Elements("shop");
                var shops = new List<Shop>();
                foreach (var shopXML in shopsXML)
                {
                    var id = shopXML.Attribute("id").Value;
                    var nameShop = shopXML.Element("name").Value;
                    var cootdinatesXML = shopXML.Element("coordinates");
                    var cootdinates = new Point(double.Parse(cootdinatesXML.Attribute("x").Value.Replace('.', ',')), double.Parse(cootdinatesXML.Attribute("y").Value.Replace('.', ',')));
                    var productsXML = shopXML.Element("products").Elements("product");
                    var products = new List<Product>();
                    foreach (var productXML in productsXML)
                    {
                        var code = productXML.Attribute("code").Value;
                        var price = float.Parse(productXML.Attribute("price").Value.Replace('.', ','));
                        var name = productXML.Value;
                        var product = new Product(code, price, name);
                        products.Add(product);
                    }
                    var shop = new Shop(id, nameShop, cootdinates, products);
                    shops.Add(shop);
                    AddLog("Add shop '" + nameShop + "'.");
                }
                this.Shops = shops.ToArray();
               
                IsLoadShops = true;
                return true;
            }
            catch
            {
                IsLoadShops = false;
                return false;
            }
        }
    }
}
