using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Shared.CQRS;

public class RequestDispatcher(IServiceProvider serviceProvider) : IRequestDispatcher
{
    public async Task<TResult> Dispatch<TRequest, TResult>(TRequest request) where TRequest : IRequest<TResult>
    {
        if (request is ICommand<TResult> command)
        {
            var handler = serviceProvider
                .GetRequiredService<ICommandHandler<ICommand<TResult>, TResult>>();
            return await handler.Handle(command, CancellationToken.None);
        }

        if (request is IQuery<TResult> query)
        {
            var handler = serviceProvider
                .GetRequiredService<IQueryHandler<IQuery<TResult>, TResult>>();
            return await handler.Handle(query, CancellationToken.None);
        }

        throw new InvalidOperationException($"Unknown request type: {typeof(TRequest).Name}");
    }
}
