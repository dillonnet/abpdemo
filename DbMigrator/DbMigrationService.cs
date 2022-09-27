using Domain.Data;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace DbMigrator;

public class DbMigrationService: ITransientDependency
{
    public ILogger<DbMigrationService> Logger { get; set; }
    public MyDbContext DbContext { get; set; }
    public IDataSeeder DataSeeder { get; set; }
    
    public async Task MigrateAsync()
    {
        Logger.LogInformation("开始数据迁移...");
        await MigrateDatabaseSchemaAsync();
        Logger.LogInformation("开始生成测试数据...");
        await SeedDataAsync();
    }
    
    private async Task MigrateDatabaseSchemaAsync()
    {
        var dataBase = DbContext.Database;
#if  DEBUG
        await dataBase.EnsureDeletedAsync();
        await dataBase.EnsureCreatedAsync();
#else 
        await dataBase.MigrateAsync();
#endif
    }
    
    private async Task SeedDataAsync()
    {
        await DataSeeder.SeedAsync();
    }
}