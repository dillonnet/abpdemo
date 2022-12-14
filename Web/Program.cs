using Web;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAutofac();
await builder.AddApplicationAsync<WebModule>();


var app = builder.Build();

await app.InitializeApplicationAsync();
await app.RunAsync();