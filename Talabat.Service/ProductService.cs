using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Service.Contract;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(specParams);

            var products = await unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            return products;

        }
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(productId);
            var product = await unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);
            return product;
        }

        public async Task<int> GetCountAsync(ProductSpecParams specParams)
        {
            var countSpec = new ProductWithFilterationForCountSpecifications(specParams);

            var count = await unitOfWork.Repository<Product>().GetCountAsync(countSpec);

            return count;
        }

        public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
            => await unitOfWork.Repository<ProductBrand>().GetAllAsync();

        public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
            => await unitOfWork.Repository<ProductCategory>().GetAllAsync();


    }
}
