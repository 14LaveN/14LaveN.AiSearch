using Persistence;

namespace AiChat.API.Persistence;

public sealed class ChatDbContextSeed(
    ILogger<ChatDbContextSeed> logger)
    : IDbSeeder<ChatDbContext>
{
    public async Task SeedAsync(ChatDbContext context)
    {
        
    }
}