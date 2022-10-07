using Application.Dto.System;
using Application.Localization.Resources;
using Application.Permissions;
using Domain;
using Domain.Entity.System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.AutoMapper;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Application;

[DependsOn(typeof(DomainModule), 
    typeof(AbpDddApplicationModule), typeof(AbpAutoMapperModule))]
public class ApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.Configurators.Add(o =>
            {
                var systemAutoMapperProfile = o.ServiceProvider.GetRequiredService<SystemAutoMapperProfile>();
                o.MapperConfiguration.AddProfile(systemAutoMapperProfile);
            });
        });

        Configure<AbpPermissionOptions>(options =>
        {
            options.ValueProviders.Add<MyPermissionValueProvider>();
        });
        
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ApplicationModule>();
        });
        
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<ApplicationResource>("zh-Han")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/Resources/System");
            
            options.Resources
                .Add<PermissionResource>("zh-Hans")
                .AddVirtualJson("/Localization/Resources/Permission");
            
            options.DefaultResourceType = typeof(ApplicationResource);
        });
    }
}