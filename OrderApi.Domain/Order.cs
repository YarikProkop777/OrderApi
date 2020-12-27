using System;

namespace OrderApi.Domain
{
    public partial class Order
    {
        public Guid Id { get; set; }

        // 2 - paid state
        public int OrderState { get; set; }

        public Guid CustomerId { get; set; }

        public string CustomerFullName { get; set; }
    }
}
