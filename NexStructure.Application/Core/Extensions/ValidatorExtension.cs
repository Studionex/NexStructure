using FluentValidation;
using NexStructure.Domain.Common.Models;

namespace NexStructure.Application.Core.Extensions;

public static class RuleBuilderExtensions
{
  public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> ruleBuilder, Error error)
  {
    ruleBuilder.WithMessage(error.Description);
    ruleBuilder.OverridePropertyName(error.Code);
    ruleBuilder.WithErrorCode(nameof(ErrorType.Validation));

    ruleBuilder.WithSeverity(Severity.Info);
    return ruleBuilder;
  }
  public static IRuleBuilder<T, string> WithLength<T>(this IRuleBuilder<T, string> ruleBuilder, Length length)
  {
    ruleBuilder = ruleBuilder.Length(length.Min, length.Max);
    return ruleBuilder;
  }
}
