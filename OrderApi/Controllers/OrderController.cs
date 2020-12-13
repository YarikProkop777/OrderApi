using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Domain;
using OrderApi.Models;

namespace OrderApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper; // _ for private members remember it!!!

        public OrderController(IMapper mapper)
        {
            _mapper = mapper;
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
                return null;
                // TODO: implement mediator
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}