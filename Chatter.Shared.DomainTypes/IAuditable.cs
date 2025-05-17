namespace Chatter.Shared.DomainTypes;

public interface IAuditable
{
    DateTime DateCreatedUtc { get; set; }
    DateTime? DateModifiedUtc { get; set; }
}