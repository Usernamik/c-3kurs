using FluentValidation;

namespace Application.Apartments.Commands;

public class CreateApartmentCommandValidator : AbstractValidator<CreateApartmentCommand>
{
    public CreateApartmentCommandValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(500);

        RuleFor(x => x.Area)
            .InclusiveBetween(1m, 100_000m);

        RuleFor(x => x.RoomsCount)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Price)
            .InclusiveBetween(0m, 100_000_000m);
    }
}
