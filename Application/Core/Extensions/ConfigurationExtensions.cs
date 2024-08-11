using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace Application.Core.Extensions;

/// <summary>
/// Represents the configuration extensions class.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Get connection string with the specified name.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="name">The name.</param>
    /// <returns>Returns result after getting connection string.</returns>
    /// <remarks>If connection string is null that throw <see cref="InvalidConfigurationException"/>.</remarks>>
    /// <exception cref="InvalidConfigurationException">The exception which is thrown when the string is null.</exception>
    public static string GetConnectionStringOrThrow(
        this IConfiguration configuration,
        string name) =>
        configuration
            .GetConnectionString(name) ??
        throw new InvalidConfigurationException(
        $"The connection string {name} was not found");
}