using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexpoint.Extensions;
using SignalR.Documentation;
using NexStructure.Application.Core.Behaviors;

namespace NexStructure.Application.Core;

public static class DependencyInjection
{
  public static void AddApplication(this IEndpointRouteBuilder api)
  {
    api.MapEndpointsFromAssembly();
    api.MapSignalRDocumentation("/realtime-docs", typeof(DependencyInjection).Assembly);


  }
  public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddNexpoint();
    services.AddSignalRDocumentation();

    services.AddMediatR(options => { options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly); });
    services.AddScoped(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehavior<,>)
    );
    services.AddScoped(
        typeof(IPipelineBehavior<,>),
        typeof(UnitOfWorkBehavior<,>)
    );
    services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);


    var assembly = typeof(DependencyInjection).Assembly;


    // var baseType = typeof(IEventHandler<>);
    //
    // var types = assembly.GetTypes().Where(t =>
    //     t is { IsAbstract: false, IsInterface: false, IsClass: true }
    //     && t.GetInterfaces().Any(i =>
    //         i.IsGenericType && i.GetGenericTypeDefinition() == baseType));
    //
    //
    // foreach (var type in types)
    // {
    //     var implementedInterface = type.GetInterfaces()
    //         .First(i => i.IsGenericType &&
    //                     i.GetGenericTypeDefinition() == baseType);
    //
    //     services.AddScoped(implementedInterface, type);
    // }
  }

  public static void AddApplication(this IServiceProvider services)
  {

    // var eventBus = services.GetRequiredService<InMemoryEventBus>();
    //
    // var assembly = typeof(Program).Assembly;
    //
    // var eventType = typeof(IAppEvent);
    //
    // var eventTypes = assembly.GetTypes()
    //     .Where(t =>
    //         t is { IsAbstract: false, IsInterface: false } &&
    //         eventType.IsAssignableFrom(t));
    //
    // var subscribeMethod = typeof(InMemoryEventBus)
    //     .GetMethod(nameof(InMemoryEventBus.Subscribe))!;
    //
    // foreach (var evt in eventTypes)
    // {
    //
    //     var genericSubscribe = subscribeMethod.MakeGenericMethod(evt);
    //
    //     Func<IAppEvent, Task> handlerDelegate = async appEvent =>
    //     {
    //         using var scope = services.CreateScope();
    //
    //         var handlerType = typeof(IEventHandler<>).MakeGenericType(evt);
    //
    //         dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);
    //
    //         await handler.Handle((dynamic)appEvent);
    //     };
    //
    //     genericSubscribe.Invoke(eventBus, [handlerDelegate]);
    // }
  }
}
