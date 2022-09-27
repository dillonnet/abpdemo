// See https://aka.ms/new-console-template for more information

using DbMigrator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder().AddAppSettingsSecretsJson().ConfigureServices((hostContext, services) =>
{
    services.AddHostedService<DbMigratorHostedService>();
}).RunConsoleAsync();

