using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderApi.Data.Repository;
using OrderApi.Domain;

namespace OrderApi.Service.v1.Query
{
    public class GetOrdersByCustomerIdQueryHandler : IRequestHandler<GetOrdersByCustomerIdQuery, List<Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByCustomerIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<Order>> Handle(GetOrdersByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            return await _orderRepository.GetOrdersByCustomerIdAsync(request.CustomerId, cancellationToken);
        }
    }
}
