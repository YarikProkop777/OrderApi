using MediatR;
using OrderApi.Domain;
using System.Collections.Generic;

namespace OrderApi.Service.v1.Query
{
    public class GetPaidOrdersQuery : IRequest<List<Order>>
    {
    }
}
