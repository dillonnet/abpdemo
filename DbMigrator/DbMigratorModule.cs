using Application;
using Domain;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ApplicationModule)
)]
public class DbMigratorModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }
}