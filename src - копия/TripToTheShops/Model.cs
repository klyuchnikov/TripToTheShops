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

        private readonly string startupTime;

        /// <summary>
        /// Внутреннее поле сущности Модели
        /// </summary>
        private static readonly Model current = new Model();

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
        private readonly List<string> log;

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
                    var products = (from productXML in productsXML
                                    let code = productXML.Attribute("code").Value
                                    let price = float.Parse(productXML.Attribute("price").Value.Replace('.', ','))
                                    let name = productXML.Value
                                    select new Product(code, price, name)).ToList();
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
                ParameterShopping op;
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
        /// <param name="startPoint"></param>
        /// <returns>словарь, ключ которого Id магазина, а значение - список продуктов из этого магазина</returns>
        public Dictionary<string, List<Product>> PlanMinimizeDist(Product[] planShopping, Point startPoint)
        {
            var variantsShops = GetVariantsShops(planShopping);
            Dictionary<string, List<Product>> minPathShops = null;
            var minPathShopsDist = 0.0;
            foreach (var shops in variantsShops)
            {
                var dictionary = shops.Keys.ToDictionary(a => a, a => this.Shops.Single(q => q.ID == a).Coordinates);
                var lastPoints = new List<Point>(dictionary.Values);
                var minPath = new Stack<Point>();
                var minPathDist = 0.0;
                var st = new Stack<Point>();
                st.Push(startPoint);

                Rec(ref minPath, ref minPathDist, st, lastPoints, startPoint);

                var newOrderListShops = new Dictionary<string, List<Product>>();
                var ss = minPath.Skip(1).TakeWhile(q => !(q.X == startPoint.X && q.Y == startPoint.Y)).ToArray();
                foreach (var a in ss)
                {
                    var keyValue = shops.Single(q => q.Key == dictionary.Single(z => z.Value == a).Key);
                    newOrderListShops.Add(keyValue.Key, keyValue.Value);
                }

                if (!(minPathDist < minPathShopsDist) && minPathShopsDist != 0.0) continue;
                minPathShops = newOrderListShops;
                minPathShopsDist = minPathDist;
                if (minPathShops.Count == 0)
                { }
            }


            return minPathShops;
        }
        /// <summary>
        /// Получение различных вариантов походов по магазинам 
        /// </summary>
        /// <param name="planShopping">план покупок</param>
        /// <returns> список набора магазинов со списком покупок</returns>
        private List<Dictionary<string, List<Product>>> GetVariantsShops(Product[] planShopping)
        {
            var variantsShops = new List<Dictionary<string, List<Product>>>();
            foreach (var p in planShopping)
            {
                var shops = this.Shops.Where(a => a.Products.Contains(p, new ProductComparer()));
                if (variantsShops.Count > 0)
                    for (var i = 0; i < variantsShops.Count; i++)
                    {
                        var s = variantsShops[i];

                        if (s.Count(a => this.Shops.Single(q => q.ID == a.Key).Products.Contains(p, new ProductComparer())) == 0)
                        {
                            var nls = new List<Dictionary<string, List<Product>>>();
                            foreach (var a in shops)
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
                                foreach (
                                    var keyValue in
                                        s.Where(
                                            sh =>
                                            this.Shops.Single(q => q.ID == sh.Key).Products.Contains(p, new ProductComparer())))
                                {
                                    keyValue.Value.Add(p);
                                    break;
                                }
                        }
                    }
                else
                {
                    var nls = shops.Select(a => new Dictionary<string, List<Product>> { { a.ID, new List<Product>(new[] { p }) } }).ToList();
                    variantsShops.AddRange(nls);
                }
            }
            return variantsShops;
        }


        /// <summary>
        /// Рекурсивная функция для получения минимального пути в графе магазинов
        /// </summary>
        /// <param name="minPath">минимальный путь Stack</param>
        /// <param name="minPathDist">минимальный путь double</param>
        /// <param name="st">очередность прохода по магазинам</param>
        /// <param name="lastPoints">оставшиеся магазины</param>
        /// <param name="startPoint">начальная точка дома</param>
        private void Rec(ref Stack<Point> minPath, ref double minPathDist, Stack<Point> st, List<Point> lastPoints, Point startPoint)
        {
            if (lastPoints.Count > 0)
                foreach (var p in lastPoints)
                {
                    var nst = new Stack<Point>(st.Reverse());
                    nst.Push(p);
                    var newLastPoints = new List<Point>(lastPoints);
                    newLastPoints.Remove(p);
                    Rec(ref minPath, ref minPathDist, nst, newLastPoints, startPoint);
                }
            else
            {
                st.Push(startPoint);
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
                var prod = this.Shops.SelectMany(q => q.Products).Where(a => a.Code == p.Code).OrderBy(q => q.Price).First();
                if (listProducts.ContainsKey(prod.Shop.ID))
                    listProducts[prod.Shop.ID].Add(prod);
                else
                    listProducts.Add(prod.Shop.ID, new List<Product>(new[] { prod }));
            }
            return listProducts;
        }

        /// <summary>
        /// Генерирование XML документа плана покупок
        /// </summary>
        /// <returns>XML документ</returns>
        public XDocument GenShoppingPlan()
        {
            var products = this.AllProducts.Where(q => this.ShoppingList.CodesProduct.Contains(q.Code)).ToArray();
            Dictionary<string, List<Product>> listProducts;
            if (this.ShoppingList.OptimizeParameter == ParameterShopping.Distance)
                listProducts = this.PlanMinimizeDist(products, this.ShoppingList.Coordinates);
            else
                listProducts = this.PlanMinimizeCost(products);
            var totalCost = 0.0;
            var totalDist = 0.0;
            totalCost += listProducts.ElementAt(0).Value.Sum(q => q.Price);
            totalDist += GetDistance(this.ShoppingList.Coordinates, this.Shops.Single(a => a.ID == listProducts.First().Key).Coordinates);
            for (var i = 0; i < listProducts.Count - 1; i++)
            {
                var shopSource = this.Shops.Single(a => a.ID == listProducts.Keys.ElementAt(i));
                var shopDest = this.Shops.Single(a => a.ID == listProducts.Keys.ElementAt(i + 1));
                totalCost += listProducts.ElementAt(i + 1).Value.Sum(q => q.Price);
                totalDist += GetDistance(shopSource.Coordinates, shopDest.Coordinates);
            }
            totalDist += GetDistance(this.Shops.Single(a => a.ID == listProducts.Last().Key).Coordinates, this.ShoppingList.Coordinates);

            var rootElem = new XElement("shoppingPlan");
            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "no"), new XDocumentType("shoppingPlan", null, "shopping-plan.dtd", null), rootElem);
            rootElem.Add(new XElement("coordinates", new XAttribute("x", this.ShoppingList.Coordinates.X),
                                      new XAttribute("y", this.ShoppingList.Coordinates.Y)));
            var totalXml = new XElement("totals");
            totalXml.Add(new XElement("cost", string.Format("{0:G7}", totalCost).Replace(',', '.')));
            totalXml.Add(new XElement("distance", string.Format("{0:G7}", totalDist).Replace(',', '.')));
            rootElem.Add(totalXml);
            var shops = new XElement("shops");
            rootElem.Add(shops);
            foreach (var shop in listProducts)
            {
                var shopXml = new XElement("shop");
                shops.Add(shopXml);
                shopXml.Add(new XAttribute("id", shop.Key));
                var s = this.Shops.Single(q => q.ID == shop.Key);
                shopXml.Add(new XElement("name", s.Name));
                var elements = new XElement("products");
                shopXml.Add(elements);
                elements.Add(shop.Value.Select(q => new XElement("product", q.Name, new XAttribute("code", q.Code))));
            }
            return doc;
        }
    }
}
