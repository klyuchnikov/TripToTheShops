using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TripToTheShops
{
    public class Shop
    {
        /// <summary>
        /// Идентификатор магазина
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Название магазина 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Координаты магазина
        /// </summary>
        public Point Coordinates { get; private set; }

        /// <summary>
        /// Список продаваемых продуктов
        /// </summary>
        public Product[] Products { get; private set; }

        /// <summary>
        /// Создание нового экземпляра магазина
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Название</param>
        /// <param name="coordinates">Координаты</param>
        /// <param name="products">Список продуктов</param>
        public Shop(string id, string name, Point coordinates, IEnumerable<Product> products)
        {
            this.ID = id;
            this.Name = name;
            this.Coordinates = coordinates;
            this.Products = products.ToArray();
            foreach (var p in this.Products)
                p.SetShop(this);
        }
    }
}
