using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using Talabat.Core.Service.Contract;

namespace Talabat.APIs.Helpers
{
    public class CashedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int timeToLive;

        public CashedAttribute(int timeToLive)
        {
            this.timeToLive = timeToLive;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responseCasheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCasheService>();

            var casheKey = GenerateCasheKeyFromRequest(context.HttpContext.Request);

            var response = await responseCasheService.GetCashedResponseAsync(casheKey);

            if (!string.IsNullOrEmpty(response))
            {
                var result = new ContentResult()
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = result;
                return;
            }

            var excutedActionContext = await next.Invoke();

            if (excutedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                await responseCasheService.CasheResponseAsync(casheKey, okObjectResult.Value, TimeSpan.FromSeconds(timeToLive));
            }
        }

        private string GenerateCasheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path);

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
                keyBuilder.Append($"|{key}-{value}");

            return keyBuilder.ToString();
        }
    }
}
