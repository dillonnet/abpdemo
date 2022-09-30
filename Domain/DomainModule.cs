using Domain.Data;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Domain;

[DependsOn(
    typeof(InfrastructureModule),
    typeof(AbpEntityFrameworkCoreModule)
    )]
public class DomainModule: AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<MyDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
        });
        
        Configure<AbpDataFilterOptions>(options =>
        {
            options.DefaultStates[typeof(IMultiTenant)] = new DataFilterState(isEnabled: false);
        });
    }
}