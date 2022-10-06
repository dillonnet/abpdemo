using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace Infrastructure;

[DependsOn(typeof(AbpCachingModule))]
public class InfrastructureModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        
    }
}