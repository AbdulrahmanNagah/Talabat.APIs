using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeed
    {
        public async static Task SeedAsync(StoreContext dbContext)
        {
            if (dbContext.ProductBrands.Count() == 0)
            {

                var brandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                if (brands?.Count() > 0)
                    foreach (var brand in brands)
                        dbContext.Set<ProductBrand>().Add(brand);

                await dbContext.SaveChangesAsync();
            }

            if (dbContext.ProductCategories.Count() == 0)
            {

                var CategoriesData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/categories.json");
                var Categories = JsonSerializer.Deserialize<List<ProductCategory>>(CategoriesData);

                if (Categories?.Count() > 0)
                    foreach (var category in Categories)
                        dbContext.Set<ProductCategory>().Add(category);

                await dbContext.SaveChangesAsync();
            }

            if (dbContext.Products.Count() == 0)
            {

                var productsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products?.Count() > 0)
                    foreach (var product in products)
                        dbContext.Set<Product>().Add(product);

                await dbContext.SaveChangesAsync();
            }

            if (dbContext.DeliveryMethods.Count() == 0)
            {

                var deliveryMethodsData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");
                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);

                if (deliveryMethods?.Count() > 0)
                    foreach (var deliveryMethod in deliveryMethods)
                        dbContext.Set<DeliveryMethod>().Add(deliveryMethod);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
