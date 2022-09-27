using Domain.Consts;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Security.Claims;

namespace Application.Permissions;

public class MyPermissionValueProvider: PermissionValueProvider
{
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

        return roles.Any(r => r == StaticRoleNames.Admin)
            ? PermissionGrantResult.Granted
            : PermissionGrantResult.Undefined;
    }

    public override Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        throw new NotImplementedException();
    }

    public override string Name => "SystemAdmin";
}