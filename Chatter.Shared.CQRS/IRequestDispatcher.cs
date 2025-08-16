namespace Chatter.Shared.CQRS;

public interface IRequestDispatcher
{
    Task<TResult> Dispatch<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);
}