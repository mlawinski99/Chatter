namespace Chatter.Shared.CQRS;

public interface ICommand<TResponse> : IRequest<TResponse>
{
    
}