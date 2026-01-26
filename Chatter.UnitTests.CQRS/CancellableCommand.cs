using Chatter.Shared.CQRS;

namespace Chatter.UnitTests.CQRS;

public class CancellableCommand : ICommand<bool>
{
}

public class CancellableCommandHandler : ICommandHandler<CancellableCommand, bool>
{
    public Task<bool> Handle(CancellableCommand command, CancellationToken cancellationToken)
    {
        return Task.FromResult(!cancellationToken.IsCancellationRequested);
    }
}