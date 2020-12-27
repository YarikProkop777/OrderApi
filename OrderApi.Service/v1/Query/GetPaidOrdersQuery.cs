using System.Collections.Generic;
using MediatR;
using OrderApi.Domain;

namespace OrderApi.Service.v1.Query
{
    public class GetPaidOrdersQuery : IRequest<List<Order>>
    {
    }
}
