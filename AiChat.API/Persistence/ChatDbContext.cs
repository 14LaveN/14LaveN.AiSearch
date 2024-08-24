using Application.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AiChat.API.Persistence;

public sealed class ChatDbContext
    : DbContext, IDbContext
{
    
}