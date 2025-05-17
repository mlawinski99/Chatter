namespace Chatter.Shared.CQRS;

public interface IRequestDispatcher
{
    Task<TResult> Dispatch<TRequest, TResult>(TRequest request)
        where TRequest : IRequest<TResult>;
}