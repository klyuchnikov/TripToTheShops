using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TripToTheShops
{
    /// <summary>
    /// Продукт, доступный в магазине
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Код товара
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Цена товара
        /// </summary>
        public float Price { get; private set; }

        /// <summary>
        /// Наименование товара
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Создание нового экземпляра товара
        /// </summary>
        /// <param name="code">Код</param>
        /// <param name="price">Цена</param>
        /// <param name="name">Наименование</param>
        public Product(string code, float price, string name)
        {
            this.Code = code;
            this.Price = price;
            this.Name = name;
        }
    }
}
