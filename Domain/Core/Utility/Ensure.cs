namespace Domain.Core.Utility;

/// <summary>
/// Contains assertions for the most common application checks.
/// </summary>
public static class Ensure
{
    /// <summary>
    /// Ensures that the specified <see cref="string"/> value is not empty.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message to show if the check fails.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <exception cref="ArgumentException"> if the specified value is empty.</exception>
    public static void NotEmpty(string value, string message, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, argumentName);
        }
    }

    /// <summary>
    /// Ensures that the specified <see cref="Ulid"/> value is not empty.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message to show if the check fails.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <exception cref="ArgumentException"> if the specified value is empty.</exception>
    public static void NotEmpty(Ulid value, string message, string argumentName)
    {
        if (value == Ulid.Empty)
        {
            throw new ArgumentException(message, argumentName);
        }
    }

    /// <summary>
    /// Ensures that the specified <see cref="DateTime"/> value is not empty.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message to show if the check fails.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <exception cref="ArgumentException"> if the specified value is the default value for the type.</exception>
    public static void NotEmpty(DateTime value, string message, string argumentName)
    {
        if (value == default)
        {
            throw new ArgumentException(message, argumentName);
        }
    }
    
    /// <summary>
    /// Ensures that the specified <see cref="int"/> value is not zero.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message to show if the check fails.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <exception cref="ArgumentException"> if the specified value is the default value for the type.</exception>
    public static void NotZero(int value, string message, string argumentName)
    {
        if (value is 0)
        {
            throw new ArgumentNullException(argumentName, message);
        }
    }

    /// <summary>
    /// Ensures that the specified value is not null.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message to show if the check fails.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <exception cref="ArgumentNullException"> if the specified value is null.</exception>
    public static void NotNull<T>(T value, string message, string argumentName)
        where T : class
    {
        if (value is null)
        {
            throw  new ArgumentNullException(argumentName, message);
        }
    }
}