using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace DbMigrator;

public class DbMigratorHostedService: IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var application = await AbpApplicationFactory.CreateAsync<DbMigratorModule>(options =>
               {
                   options.UseAutofac();
               }))
        {
            await application.InitializeAsync();
            
            await application
                .ServiceProvider
                .GetRequiredService<DbMigrationService>()
                .MigrateAsync();
            
            await application.ShutdownAsync();
            _hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}