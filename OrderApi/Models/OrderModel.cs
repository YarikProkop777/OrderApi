using System;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models
{
    public class OrderModel
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public string CustomerFullName { get; set; }
    }
}
