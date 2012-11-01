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
        public string Code { get; set; }

        /// <summary>
        /// Цена товара
        /// </summary>
        public float Price { get; set; }

        /// <summary>
        /// Наименование товара
        /// </summary>
        public string Name { get; set; }
    }
}
