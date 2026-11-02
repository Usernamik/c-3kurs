using FluentValidation;

namespace Application.Products.Commands;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(x => x.Price)
            .InclusiveBetween(0.01m, 1_000_000m);
    }
}
