using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Service.Contract;

namespace Talabat.APIs.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentsController> logger;
        private const string whSecret = "whsec_ef7ab9f990731b4c87ba94e2601d9b70daad50812d4816941d12d058d86bf77b";

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            this.paymentService = paymentService;
            this.logger = logger;
        }


        [Authorize]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket == null) return BadRequest(new ApiResponse(400, "An Error With Your Basket"));
            return Ok(basket);
        }



        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], whSecret);

            var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

            Order order;

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    order = await paymentService.UpdatePaymentIntentToSuccededOrFaild(paymentIntent.Id, true);
                    logger.LogInformation("Payment is succeded :)", paymentIntent.Id);
                    break;
                case Events.PaymentIntentPaymentFailed:
                    order = await paymentService.UpdatePaymentIntentToSuccededOrFaild(paymentIntent.Id, false);
                    logger.LogInformation("Payment is faild :(", paymentIntent.Id);
                    break;
                default:
                    break;
            }
    
            return Ok();
        }
    }
}
