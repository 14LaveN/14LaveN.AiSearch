using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Domain.Core.Utility;

namespace Domain.Core.Primitives;

/// <summary>
/// Represents the generic mongo entity class.
/// </summary>
public abstract class BaseMongoEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseMongoEntity"/> class.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    protected BaseMongoEntity(string id)
        : this()
    {
        Ensure.NotEmpty(id, "The identifier is required.", nameof(id));

        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseMongoEntity"/> class.
    /// </summary>
    /// <remarks>
    /// Required by EF Core.
    /// </remarks>
    protected BaseMongoEntity()
    {
    }
    
    /// <summary>
    /// Gets or sets identifier.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets date/time created at.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static bool operator ==(BaseMongoEntity a, BaseMongoEntity b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(BaseMongoEntity a, BaseMongoEntity b) => !(a == b);

    /// <inheritdoc />
    public bool Equals(BaseMongoEntity other)
    {
        if (other is null)
        {
            return false;
        }

        return ReferenceEquals(this, other) || Id == other.Id;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        if (!(obj is BaseMongoEntity other))
        {
            return false;
        }

        if (Id == string.Empty || other.Id == string.Empty)
        {
            return false;
        }

        return Id == other.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id!.GetHashCode() * 41;
}