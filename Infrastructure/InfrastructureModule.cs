using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;

namespace Infrastructure;

[DependsOn(typeof(AbpCachingModule))]
public class InfrastructureModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Local;
        });
    }
}