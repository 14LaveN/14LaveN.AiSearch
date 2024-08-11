using System.Net;
using Domain.Common.Core.Primitives.Result;
using Domain.Core.Primitives.Result;

namespace Application.ApiHelpers.Responses;

/// <summary>
/// Represents the base response interface.
/// </summary>
/// <typeparam name="T">The generic type.</typeparam>
public interface IBaseResponse<T>
{
    /// <summary>
    /// Gets or sets status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Gets or sets description.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Gets or sets data.
    /// </summary>
    public Result<T> Data { get; set; }
}