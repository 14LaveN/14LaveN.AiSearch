using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultAuthentication();

var app = builder.Build();


app.Run();