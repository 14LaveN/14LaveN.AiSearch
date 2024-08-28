using System.Net;
using System.Text;
using System.Text.Json;
using Domain.Common.Core.Primitives;
using Domain.Core.Primitives.Result;
using Identity.API.Domain.Entities;

namespace SearchingService.API.Apis;

public sealed class ElasticClient(HttpClient httpClient)
{
    public async Task<Result> AddUserToDocument(
        string json,
        string indexName,
        CancellationToken cancellationToken = default)
    {
        using JsonDocument document = JsonDocument.Parse(json);
            
        string? userName = document.RootElement.GetProperty("UserName").GetString();
        
        HttpResponseMessage response = await httpClient.PostAsync(
            $"http://localhost:9200/{indexName}/_doc/{userName}",
            new StringContent(json ,Encoding.UTF8, "application/json"),
            cancellationToken);

        return await (response.StatusCode == HttpStatusCode.Created
            ? Result.Success()
            : Result.Failure(new Error(response.StatusCode.ToString(), response.Content.ToString())));
    }
}