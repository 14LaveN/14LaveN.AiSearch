using Microsoft.AspNetCore.Routing;

namespace Application.Core.Abstractions.Endpoint;

/// <summary>
/// Represents the endpoint interface.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Map the some endpoint.
    /// </summary>
    /// <param name="app">The application builder.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}