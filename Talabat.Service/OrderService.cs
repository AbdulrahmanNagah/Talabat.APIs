using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Repository.Contract;
using Talabat.Core.Service.Contract;
using Talabat.Core.Specifications.Order_Specs;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository basketRepo;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPaymentService paymentService;


        public OrderService(
            IBasketRepository basketRepo,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService
        {
            this.basketRepo = basketRepo;
            this.unitOfWork = unitOfWork;
            this.paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            // 1. Get basket from Baskets Repo
            var basket = await basketRepo.GetBasketAsync(basketId);

            // 2. Get Selected items at basket from Products Repo

            var orderItems = new List<OrderItem>();

            if (basket?.Items?.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var productRepository = unitOfWork.Repository<Product>();
                    var product = await productRepository.GetByIdAsync(item.Id);

                    var productItemOrdered = new ProductItemOrdered(item.Id, product.Name, product.PictureUrl);

                    var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                    orderItems.Add(orderItem);
                }
            }

            // 3. Calculate Subtotal

            var subtotal = orderItems.Sum(oi => oi.Price *  oi.Quantity);

            // 4. Get Delivery method from deliveryMethods Repo

            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            var ordersRepo = unitOfWork.Repository<Order>();

            var orderSpec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);

            var existingOrder = await ordersRepo.GetEntityWithSpecAsync(orderSpec);

            if (existingOrder != null)
            {
                ordersRepo.Delete(existingOrder);

                await paymentService.CreateOrUpdatePaymentIntent(basketId);
            }

            // 5. Create Order

            var order = new Order (buyerEmail, shippingAddress, deliveryMethod, orderItems, subtotal, basket.PaymentIntentId);

            await unitOfWork.Repository<Order>().AddAsync(order);

            // 6. Save To Database 

            var result = await unitOfWork.CompleteAsync();

            if (result <= 0)
                return null;

            return order;


        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var ordersRepo =  unitOfWork.Repository<Order>();

            var spec = new OrderSpecifications(buyerEmail);

            var orders = await ordersRepo.GetAllWithSpecAsync(spec);

            return orders;
        }

        public async Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var orderRepo = unitOfWork.Repository<Order>();

            var orderSpec = new OrderSpecifications(orderId, buyerEmail);

            var order = await orderRepo.GetEntityWithSpecAsync(orderSpec);

            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
            => await unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
    }
}
