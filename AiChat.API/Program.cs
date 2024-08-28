using HigLabo.OpenAI;
using HuggingFace;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

//TODO The chat gpt requires cash.

builder.AddDefaultAuthentication();

var app = builder.Build();

app.Run();