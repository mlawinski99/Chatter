namespace Chatter.Shared.DomainTypes;

public abstract class Entity
{
    public Guid Id { get; set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    public override bool Equals(object obj)
    {
        if (obj is not Entity other) return false;
        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
}