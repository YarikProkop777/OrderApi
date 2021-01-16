using FluentValidation;
using OrderApi.Models;

namespace OrderApi.Validators
{
    public class OrderModelValidator : AbstractValidator<OrderModel>
    {
        public OrderModelValidator()
        {
            RuleFor(x => x.CustomerFullName)
                .NotNull()
                .WithMessage("The customer name must not be empty");

            RuleFor(x => x.CustomerFullName)
                .MinimumLength(2)
                .WithMessage("The customer name must be at least 2 character long");
        }
    }
}
