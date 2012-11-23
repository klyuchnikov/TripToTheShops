using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TripToTheShops
{
    /// <summary>
    /// Критерий оптимизации
    /// </summary>
    public enum ParameterShopping
    {
        /// <summary>
        /// Оптимизация по общей стоимости
        /// </summary>
        Cost = 0,
        /// <summary>
        /// Оптимизация по общему расстоянию
        /// </summary>
        Distance = 1
    }
}
