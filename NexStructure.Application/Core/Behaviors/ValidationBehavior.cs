using FluentValidation;
using MediatR;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace NexStructure.Application.Core.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
  public async Task<TResponse> Handle(
      TRequest request,
      RequestHandlerDelegate<TResponse> next,
      CancellationToken cancellationToken)
  {
    if (validator is null) return await next(cancellationToken);

    var validatorResult = await validator.ValidateAsync(request, cancellationToken);

    if (validatorResult.IsValid) return await next(cancellationToken);

    var errors = ValidationBehavior<TRequest, TResponse>.MapValidatorErrors(validatorResult.Errors);
    return (dynamic)errors;
  }

  private static List<Error> MapValidatorErrors(List<ValidationFailure> validatorFailures)
  {
    var errors = new List<Error>();
    foreach (var validatorFailure in validatorFailures)
    {
      var error = ValidationBehavior<TRequest, TResponse>.MapErrorCodeToErrorType(validatorFailure);
      errors.Add(error);
    }

    return errors;
  }

  private static Error MapErrorCodeToErrorType(ValidationFailure error)
  {
    return error.ErrorCode switch
    {
      "Failure" => Error.Failure(error.PropertyName, error.ErrorMessage),
      "Unexpected" => Error.Unexpected(error.PropertyName, error.ErrorMessage),
      "Validation" => Error.Validation(error.PropertyName, error.ErrorMessage),
      "Conflict" => Error.Conflict(error.PropertyName, error.ErrorMessage),
      "NotFound" => Error.NotFound(error.PropertyName, error.ErrorMessage),
      "Unauthorized" => Error.Unauthorized(error.PropertyName, error.ErrorMessage),
      "Forbidden" => Error.Forbidden(error.PropertyName, error.ErrorMessage),
      _ => Error.Validation(error.PropertyName, error.ErrorMessage)
    };
  }
}
