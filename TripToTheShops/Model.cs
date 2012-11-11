using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows;
using System.IO;

namespace TripToTheShops
{
    /// <summary>
    /// Класс модели 
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Закрытый конструктор
        /// </summary>
        protected Model()
        {
            this.log = new List<string>();
            this.Shops = new Shop[0] { };
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
        /// Список покупок
        /// </summary>
        public ShoppingList ShoppingList { get; private set; }

        /// <summary>
        /// Все продукты
        /// </summary>
        public Product[] AllProducts
        {
            get
            {
                return Shops.SelectMany(q => q.Products).Distinct(new ProductComparer()).ToArray();
            }
        }

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
        /// <param name="path">XML документ</param>
        /// <returns>Загружен ли список магазинов</returns>
        public bool LoadShops(string path)
        {
            try
            {
                var doc = XDocument.Load(path);
                var shopsXML = doc.Root.Elements("shop").ToArray();
                var shops = new List<Shop>();
                if (shopsXML.Length == 0)
                    throw new FileLoadException("Error read file.");
                foreach (var shopXML in shopsXML)
                {
                    var id = shopXML.Attribute("id").Value;
                    var nameShop = shopXML.Element("name").Value;
                    var coordinatesXML = shopXML.Element("coordinates");
                    var coordinates = new Point(double.Parse(coordinatesXML.Attribute("x").Value.Replace('.', ',')), double.Parse(coordinatesXML.Attribute("y").Value.Replace('.', ',')));
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
                    var shop = new Shop(id, nameShop, coordinates, products);
                    shops.Add(shop);
                    AddLog("Add shop '" + nameShop + "'.");
                }
                this.Shops = shops.ToArray();

                IsLoadShops = true;
                return true;
            }
            catch (Exception e)
            {
                AddLog(e.Message);
                this.Shops = new Shop[] { };
                IsLoadShops = false;
                return false;
            }
        }

        /// <summary>
        /// Загружен ли список покупок
        /// </summary>
        public bool IsLoadShoppingList { get; private set; }

        /// <summary>
        /// Загрузка списка покупок
        /// </summary>
        /// <param name="path">XML документ</param>
        /// <returns>Загружен ли список покупок</returns>
        public bool LoadShoppingList(string path)
        {
            try
            {
                var doc = XDocument.Load(path);
                var coordinatesXML = doc.Root.Element("coordinates");
                Point coordinates = new Point(double.Parse(coordinatesXML.Attribute("x").Value.Replace('.', ',')), double.Parse(coordinatesXML.Attribute("y").Value.Replace('.', ',')));
                ParameterShopping op = ParameterShopping.Cost;
                if (doc.Root.Element("parameters").Element("distance") == null)
                    if (doc.Root.Element("parameters").Element("cost") == null)
                        throw new ArgumentException("Parameter 'ParameterShopping' is not specified.");
                    else
                        op = ParameterShopping.Cost;
                else
                    op = ParameterShopping.Distance;
                var codeProducts = doc.Root.Element("products").Elements("product").Select(q => q.Attribute("code").Value).ToList();
                this.ShoppingList = new ShoppingList(coordinates, op, codeProducts);
                IsLoadShoppingList = true;
                return true;
            }
            catch (Exception e)
            {
                AddLog(e.Message);
                this.ShoppingList = null;
                IsLoadShoppingList = false;
                return false;
            }
        }
        /// <summary>
        /// Получение расстояния между двумя точками
        /// </summary>
        /// <param name="source">Начальная точка</param>
        /// <param name="destination">Конечная точка</param>
        /// <returns>Расстояние между точками</returns>
        public double GetDistance(Point source, Point destination)
        {
            return Math.Abs(source.X - destination.X) + Math.Abs(source.Y - destination.Y);
        }

        /// <summary>
        /// Получение плана покупок, оптимизирированного по дистанции до магазинов
        /// </summary>
        /// <param name="planShopping">список продуктов</param>
        /// <returns>словарь, ключ которого Id магазина, а значение - список продуктов из этого магазина</returns>
        public Dictionary<string, List<Product>> PlanMinimizeDist(Product[] planShopping)
        {
            var variantsShops = new List<Dictionary<string, List<Product>>>(); //new List<Dictionary<string, List<Product>>>();
            var remainingProducts = new List<Product>(planShopping);
            foreach (var p in planShopping)
            {
                var shopss = Model.Current.Shops.Where(a => a.Products.Contains(p, new ProductComparer()));
                if (variantsShops.Count > 0)
                    for (int i = 0; i < variantsShops.Count; i++)
                    {
                        var s = variantsShops[i];

                        if (s.Count(a => this.Shops.Single(q => q.ID == a.Key).Products.Contains(p, new ProductComparer())) == 0)
                        {
                            var nls = new List<Dictionary<string, List<Product>>>();
                            foreach (var a in shopss)
                            {
                                var ls = new Dictionary<string, List<Product>>(s);
                                if (!ls.ContainsKey(a.ID))
                                    ls.Add(a.ID, new List<Product>(new[] { p }));
                                nls.Add(ls);
                            }
                            variantsShops.RemoveAt(i--);
                            variantsShops.AddRange(nls);
                        }
                        else
                        {
                            if (!s.SelectMany(q => q.Value).Contains(p, new ProductComparer()))
                                foreach (var sh in s)
                                {
                                    if (Model.Current.Shops.Single(q => q.ID == sh.Key).Products.Contains(p, new ProductComparer()))
                                    {
                                        sh.Value.Add(p);
                                        break;
                                    }
                                }
                        }
                    }
                else
                {
                    var nls = new List<Dictionary<string, List<Product>>>();
                    foreach (var a in shopss)
                    {
                        var ls = new Dictionary<string, List<Product>>();
                        ls.Add(a.ID, new List<Product>(new[] { p }));
                        nls.Add(ls);
                    }
                    variantsShops.AddRange(nls);
                }
            }
            Dictionary<string, List<Product>> minPathShops = null;
            var minPathShopsDist = 0.0;
            foreach (var shops in variantsShops)
            {
                var dictionary = new Dictionary<string, Point>();
                foreach (var a in shops.Keys)
                    dictionary.Add(a, this.Shops.Single(q => q.ID == a).Coordinates);
                var lastPoints = new List<Point>(dictionary.Values);
                Stack<Point> minPath = new Stack<Point>();
                var minPathDist = 0.0;
                var st = new Stack<Point>();
                st.Push(new Point(0, 0));

                rec(ref minPath, ref minPathDist, st, lastPoints);

                var newOrderListShops = new Dictionary<string, List<Product>>();
                foreach (var a in minPath.Skip(1).TakeWhile(q => q.X != 0 && q.Y != 0))
                {
                    var keyValue = shops.Single(q => q.Key == dictionary.Single(z => z.Value == a).Key);
                    newOrderListShops.Add(keyValue.Key, keyValue.Value);
                }

                if (minPathDist < minPathShopsDist || minPathShopsDist == 0.0)
                {
                    minPathShops = newOrderListShops;
                    minPathShopsDist = minPathDist;
                }
            }

            return minPathShops;
        }

        private void rec(ref Stack<Point> minPath, ref double minPathDist, Stack<Point> st, List<Point> lastPoints)
        {
            if (lastPoints.Count > 0)
                foreach (var p in lastPoints)
                {
                    var nst = new Stack<Point>(st.Reverse());
                    nst.Push(p);
                    var newLastPoints = new List<Point>(lastPoints);
                    newLastPoints.Remove(p);
                    rec(ref minPath, ref minPathDist, nst, newLastPoints);
                }
            else
            {
                st.Push(new Point(0, 0));
                var total = 0.0;
                for (var i = 0; i < st.Count - 1; i++)
                    total += GetDistance(st.ElementAt(i), st.ElementAt(i + 1));
                if (total < minPathDist || minPathDist == 0.0)
                {
                    minPath = st;
                    minPathDist = total;
                }
            }
        }


        /// <summary>
        /// Получение плана покупок, оптимизирированного по дистанции до магазинов
        /// </summary>
        /// <param name="planShopping">список продуктов</param>
        /// <returns>словарь, ключ которого Id магазина, а значение - список продуктов из этого магазина</returns>
        public Dictionary<string, List<Product>> PlanMinimizeCost(Product[] planShopping)
        {
            var listProducts = new Dictionary<string, List<Product>>();
            foreach (var p in planShopping)
            {
                var prod = Model.Current.Shops.SelectMany(q => q.Products).Where(a => a.Code == p.Code).OrderBy(q => q.Price).First();
                if (listProducts.ContainsKey(prod.Shop.ID))
                    listProducts[prod.Shop.ID].Add(prod);
                else
                    listProducts.Add(prod.Shop.ID, new List<Product>(new[] { prod }));
            }
            return listProducts;
        }


    }
}
