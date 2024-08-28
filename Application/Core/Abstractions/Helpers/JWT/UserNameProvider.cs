namespace Application.Core.Abstractions.Helpers.JWT;

/// <summary>
/// Represents the username provider interface.
/// </summary>
public interface IUserNameProvider
{
    /// <summary>
    /// Gets <see cref="UserName"/>.
    /// </summary>
    public string? UserName { get; }
}