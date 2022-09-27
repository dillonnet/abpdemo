using Application.Dto.User;
using Application.Permissions;
using Domain;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Application;

[DependsOn(typeof(DomainModule))]
public class ApplicationModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<UserAutoMapperProfile>(validate: true);
        });
        
        Configure<AbpPermissionOptions>(options =>
        {
            options.ValueProviders.Add<MyPermissionValueProvider>();
        });
    }
}