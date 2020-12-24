﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderApi.Domain;
using OrderApi.Data;
using OrderApi.Data.Repository;

namespace OrderApi.Service.v1.Command
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            return await _orderRepository.AddAsync(request.Order);
        }
    }
}
