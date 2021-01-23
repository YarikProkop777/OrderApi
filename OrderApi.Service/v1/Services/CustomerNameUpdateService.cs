using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MediatR;
using OrderApi.Service.v1.Command;
using OrderApi.Service.v1.Models;
using OrderApi.Service.v1.Query;

namespace OrderApi.Service.v1.Services
{
    public class CustomerNameUpdateService : ICustomerNameUpdateService
    {
        private readonly IMediator _mediator;

        public CustomerNameUpdateService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async void UpdateCustomerNameInOrders(UpdateCustomerFullNameModel updateCustomerFullNameModel)
        {
            try
            {
                var orders = await _mediator.Send(new GetOrdersByCustomerIdQuery
                {
                    CustomerId = updateCustomerFullNameModel.Id
                });

                if (orders.Count > 0)
                {
                    orders.ForEach(x => x.CustomerFullName = $"{updateCustomerFullNameModel.FirstName} {updateCustomerFullNameModel.LastName}");

                    await _mediator.Send(new UpdateOrderCommand
                    {
                        Orders = orders
                    });
                }
            }
            catch (Exception ex)
            {
                // log an error message here

                Debug.WriteLine(ex.Message);
            }
        }
    }
}
