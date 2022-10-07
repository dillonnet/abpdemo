using Application.Service.System;
using Domain.Consts;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace Application.Permissions;

public class MyPermissionValueProvider: PermissionValueProvider
{
    public UserService UserService { get; set; }
    public MyPermissionValueProvider(IPermissionStore permissionStore)
        : base(permissionStore)
    {
    }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        var roles = context.Principal?.FindAll(AbpClaimTypes.Role).Select(c => c.Value).ToArray();

        if (roles == null || !roles.Any())
        {
            return  PermissionGrantResult.Undefined;
        }

        if (roles.Any(r => r == StaticRoleNames.Admin))
        {
            return PermissionGrantResult.Granted;
        }

        var userInfo = await UserService.GetUserInfo();
        if(userInfo.Permissions.Contains(context.Permission.Name)){
            return PermissionGrantResult.Granted;
        }
        
        return  PermissionGrantResult.Prohibited;
    }

    public override Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        throw new NotImplementedException();
    }

    public override string Name => "SystemAdmin";
}