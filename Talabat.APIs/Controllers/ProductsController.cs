using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Service.Contract;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductService productService;

        //private readonly IGenericRepository<Product> productRepo;
        //private readonly IGenericRepository<ProductBrand> brandRepo;
        //private readonly IGenericRepository<ProductCategory> categoryRepo;
        private readonly IMapper mapper;

        public ProductsController(
            IProductService productService
            //IGenericRepository<Product> productRepo,
            //IGenericRepository<ProductBrand> brandRepo,
            //IGenericRepository<ProductCategory> categoryRepo
            , IMapper mapper)
        {
            this.productService = productService;
            //this.productRepo = productRepo;
            //this.brandRepo = brandRepo;
            //this.categoryRepo = categoryRepo;
            this.mapper = mapper;
        }

        [CashedAttribute(600)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {

            var products = await productService.GetProductsAsync(specParams);

            var count = await productService.GetCountAsync(specParams);

            var data = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize,count, data));
        }

        [CashedAttribute(600)]
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ProductToReturnDto>>> GetProduct(int id)
        {
            var product = await productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound(new ApiResponse(404));

            return Ok(mapper.Map<Product, ProductToReturnDto>(product));
        }

        [CashedAttribute(600)]
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> GetAllBrands()
        {
            var brands = await productService.GetBrandsAsync();
            return Ok(brands);
        }

        [CashedAttribute(600)]
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetAllCategories()
        {
            var categories = await productService.GetCategoriesAsync();
            return Ok(categories);
        }
    }
}
