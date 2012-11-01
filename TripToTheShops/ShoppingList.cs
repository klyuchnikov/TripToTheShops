using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TripToTheShops
{
    public class ShoppingList
    {
        /// <summary>
        /// Начальная точка (местоположение покупателя)
        /// </summary>
        public Point Coordinates { get; private set; }

        /// <summary>
        /// Критерий оптимизации
        /// </summary>
        public ParameterShopping OptimizeParameter { get; private set; }

        /// <summary>
        /// Коды товаров для покупки
        /// </summary>
        public String[] CodesProduct { get; private set; }


        public ShoppingList(Point coordinates, ParameterShopping optimizeParameter, IEnumerable<string> codesProduct)
        {
            this.Coordinates = coordinates;
            this.OptimizeParameter = optimizeParameter;
            this.CodesProduct = codesProduct.ToArray();
        }

    }
}
