using System;
using System.Collections.Generic;
using MediatR;
using OrderApi.Domain;

namespace OrderApi.Service.v1.Query
{
    public class GetOrdersByCustomerIdQuery : IRequest<List<Order>>
    {
        public Guid CustomerId { get; set; }
    }
}
