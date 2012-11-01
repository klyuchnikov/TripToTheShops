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

        public Shop Shop { get; private set; }

        public void SetShop(Shop shop)
        {
            this.Shop = shop;
        }
    }

    // Custom comparer for the Product class
    class ProductComparer : IEqualityComparer<Product>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(Product x, Product y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.Code == y.Code;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(Product product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Code field.
            int hashProductCode = product.Code.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductCode;
        }

    }

}
