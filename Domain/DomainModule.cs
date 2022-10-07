using Domain.Data;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Guids;
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
        Action<IAbpDbContextRegistrationOptionsBuilder> dbContextOptionBuild = options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        };
        context.Services.AddAbpDbContext<MyDbContext>(dbContextOptionBuild);
        var dbContextRegistrationOptions = new AbpDbContextRegistrationOptions(typeof(MyDbContext), context.Services);
        dbContextOptionBuild.Invoke(dbContextRegistrationOptions);
        new MyEfCoreRepositoryRegistrar(dbContextRegistrationOptions).AddRepositories();

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
        });
        
        Configure<AbpDataFilterOptions>(options =>
        {
            options.DefaultStates[typeof(IMultiTenant)] = new DataFilterState(isEnabled: false);
        });
        
        Configure<AbpSequentialGuidGeneratorOptions>(options =>
        {
            options.DefaultSequentialGuidType = SequentialGuidType.SequentialAsString ;
        });
    }
}