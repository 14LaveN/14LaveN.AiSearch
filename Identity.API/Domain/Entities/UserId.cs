namespace Identity.API.Domain.Entities;

/// <summary>
/// Represents the user identifier record.
/// </summary>
/// <param name="Value">The guid value.</param>
public record UserId(Guid Value)
{
    /// <inheritdoc />
    public override string ToString() =>
        Value.ToString();

    /// <inheritdoc />
    public override int GetHashCode() => 
        Value.GetHashCode();

    /// <summary>
    /// Create the <see cref="Guid"/> value with specified <see cref="UserId"/> record.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Returns new <see cref="Guid"/> value.</returns>
    public static implicit operator Guid(UserId userId) => 
        userId.Value;

    /// <summary>
    /// Create the <see cref="UserId"/> value with specified <see cref="Guid"/> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Returns new <see cref="UserId"/> record.</returns>
    public static explicit operator UserId(Guid value) =>
        new(value);
}