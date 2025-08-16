using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Shared.CQRS;

public class RequestDispatcher(IServiceProvider serviceProvider) : IRequestDispatcher
{
    public async Task<TResult> Dispatch<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;

        if (request is ICommand<TResult> command)
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
            dynamic handler = provider.GetRequiredService(handlerType);
            return await handler.Handle((dynamic)command, cancellationToken);
        }

        if (request is IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = provider.GetRequiredService(handlerType);
            return await handler.Handle((dynamic)query, cancellationToken);
        }

        throw new InvalidOperationException($"Unknown request type: {request.GetType().Name}");
    }
}
