using MediatR;
using Nexpoint;
using Nexpoint.Extensions;
using Nexpoint.Models;

namespace NexStructure.Application.Core;


public class FakeEndpoint : NexEndpoint
{
    public override string Route => "/fake";

    public override List<Error> PossibleErrors =>
    [
        Error.Conflict("FakeConflict", "Already exists"),
    ];

    public override void Register()
    {
        this.MapPost<FakeCommand>(() => NexResult.Ok("ok"));
    }
}
public class FakeCommand : IRequest<ErrorOr<Unit>>;