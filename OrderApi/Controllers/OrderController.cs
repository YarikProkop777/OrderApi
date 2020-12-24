using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Domain;
using OrderApi.Models;
using OrderApi.Service.v1.Command;
using OrderApi.Service.v1.Query;

namespace OrderApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper; // _ for private members remember it!!!
        private readonly IMediator _mediator;


        public OrderController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        ///     Action to create a new order in the database
        /// </summary>
        /// <param name="order">Model to create a new order</param>
        /// <returns>Returns the created order</returns>
        /// <response code="200">>Returned if the order was created</response>
        /// <response code="400">Returned if the model couldn't be parsed or saved</response>
        /// <response code="422">Returned when the validations failed</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPost]
        public async Task<ActionResult<Order>> Order(OrderModel order)
        {
            try
            {
                return await _mediator.Send(new CreateOrderCommand
                {
                    Order = _mapper.Map<Order>(order)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Action to retrieve all pay orders.
        /// </summary>
        /// <returns>Returns a list of all paid orders or an empty list, if no order is paid yet</returns>
        /// <response code="200">Returned if the list of orders was retrieved</response>
        /// <response code="400">Returned if the orders could not be retrieved</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetPaidOrders()
        {
            try
            {
                return await _mediator.Send(new GetPaidOrdersQuery());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Action to pay an order.
        /// </summary>
        /// <param name="id">The id of the order which got paid</param>
        /// <returns>Returns the paid order</returns>
        /// <response code="200">Returned if the order was updated (paid)</response>
        /// <response code="400">Returned if the order could not be found with the provided id</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("Pay/{id}")]
        public async Task<ActionResult<Order>> Pay(Guid id)
        {
            try
            {
                var order = await _mediator.Send(new GetOrderByIdQuery
                {
                    Id = id
                });

                if (order == null)
                {
                    return BadRequest($"No order found with the id {id}");
                }

                // set paidState to order
                order.OrderState = 2;

                return await _mediator.Send(new PayOrderCommand
                {
                    Order = _mapper.Map<Order>(order)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}