namespace ZSocialMedia.Shared;

public interface IEntityBase<T>
{
    T Id { get; set; }
}

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}

public interface IAggregateRoot
{

}

public abstract class EntityBase<T> : IEntityBase<T>
    where T : IEquatable<T>
{
    public required T Id  { get; set; }
}

public abstract class EntityAggregateBase<T> : HasDomainEventBase, IEntityBase<T>, IAggregateRoot
    where T : IEquatable<T>
{
    public required T Id { get; set; }
}

public static class  EntityExtensions
{
    public static string? GetEntityIdName(this Type type)
    {
        if (!type.IsSubclassOf(typeof(EntityBase<>))) return null;
        var idProperty = type.GetProperty("Id");
        if (idProperty == null) return null;
        var prefix = type.Name.Replace("Entity", "");
        return prefix + idProperty.Name;
    }
}