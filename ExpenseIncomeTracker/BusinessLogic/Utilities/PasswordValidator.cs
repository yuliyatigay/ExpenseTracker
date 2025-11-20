using BusinessLogic.Extensions;
using Domain.Models;
using FluentValidation;

namespace BusinessLogic.Utilities;

public class PasswordValidator : AbstractValidator<Account>
{
    public PasswordValidator()
    {
        RuleFor(x => x.PasswordHash).PasswordRules(8);
    }
}