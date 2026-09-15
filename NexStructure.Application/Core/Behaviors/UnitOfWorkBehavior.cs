using MediatR;
using NexStructure.Domain.Common.Repositories;

namespace NexStructure.Application.Core.Behaviors;

public class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWorkRepository unitOfWork) :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (IsNotCommand()) return await next(cancellationToken);
        var response = await next(cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return response;
    }

    private static bool IsNotCommand()
    {
        return !typeof(TRequest).Name.EndsWith("Command");
    }
}
