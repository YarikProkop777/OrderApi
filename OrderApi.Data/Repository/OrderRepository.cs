using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data.Database;
using OrderApi.Domain;

namespace OrderApi.Data.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext orderContext) : base(orderContext)
        {
        }

        public async Task<Order> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            return await orderContext.Order.FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
        {
            return await orderContext.Order.Where(order => order.CustomerId == customerId).ToListAsync(cancellationToken);
        }

        public async Task<List<Order>> GetPaidOrdersAsync(CancellationToken cancellationToken)
        {
            return await orderContext.Order.Where(order => order.OrderState == 2).ToListAsync(cancellationToken);
        }
    }
}
