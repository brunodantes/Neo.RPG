using FluentValidation;
using NeoRPG.Contract.DTO;

namespace NeoRPG.Application.Validators;
public class BattleRequestValidator : AbstractValidator<BattleRequestDTO>
{
    public BattleRequestValidator()
    {
        RuleFor(x => x.FirstPlayer)
            .NotEmpty().WithMessage("First character id is required.");

        RuleFor(x => x.SecondPlayer)
            .NotEmpty().WithMessage("Second character id is required.");

        RuleFor(x => x)
            .Must(x => x.FirstPlayer != x.SecondPlayer)
            .WithMessage("Characters in a battle must be different.");
    }
}
