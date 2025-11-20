using FluentValidation;

namespace BusinessLogic.Extensions;

public static class PasswordRuleBuilderExtensions
{
    public static void PasswordRules<T>(this IRuleBuilder<T, string> ruleBuilder, int minLength)
    {
        ruleBuilder
            .MinimumLength(minLength)
            .WithMessage($"Password must be at least {minLength} characters long")
            .Matches("[a-z]")
            .WithMessage($"Password must contain at least 1 lowercase letter")
            .Matches("[A-Z]")
            .WithMessage($"Password must contain at least 1 uppercase letter")
            .Matches("[0-9]")
            .WithMessage($"Password must contain at least 1 number")
            .Matches(@"[^a-zA-Z0-9]")
            .WithMessage($"Password must contain at least 1 special character");
    }
}