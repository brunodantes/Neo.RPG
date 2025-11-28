using FluentValidation;
using NeoRPG.Contract.DTO;

namespace NeoRPG.Application.Validators;
public class CreateCharacterRequestValidator : AbstractValidator<CreateCharacterDTO>
{
    public CreateCharacterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(4, 15).WithMessage("Name must be between 4 and 15 characters.")
            .Matches("^[A-Za-z_]+$").WithMessage("Name can only contain letters and underscore.");

        RuleFor(x => x.Job)
            .IsInEnum().WithMessage("Job must be a valid value.");
    }
}
