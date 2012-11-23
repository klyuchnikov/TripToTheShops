using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TripToTheShops
{
    /// <summary>
    /// Список покупок, план оптимизации и начальная координата
    /// </summary>
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

        /// <summary>
        /// Создание нового экземпляра класса 
        /// </summary>
        /// <param name="startСoordinate">Начальная координата</param>
        /// <param name="optimizeParameter">Параметр оптимизации</param>
        /// <param name="codesProduct">Коды продуктов</param>
        public ShoppingList(Point startСoordinate, ParameterShopping optimizeParameter, IEnumerable<string> codesProduct)
        {
            this.Coordinates = startСoordinate;
            this.OptimizeParameter = optimizeParameter;
            this.CodesProduct = codesProduct.ToArray();
        }

    }
}
