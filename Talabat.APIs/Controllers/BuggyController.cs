using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext dbcontext;

        public BuggyController(StoreContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet("serverError")]
        public ActionResult GetServerError()
        {
            var product = dbcontext.Products.Find(100);
            var productToReturn = product.ToString();

            return Ok(productToReturn);
        }
    }
}
