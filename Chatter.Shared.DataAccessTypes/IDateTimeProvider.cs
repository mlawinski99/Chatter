namespace Chatter.Shared.DataAccessTypes;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}