using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Localization;

namespace Application.Permissions;

public class MyPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var systemGroup = context.AddGroup(MyPermissions.SystemGroupName, L("系统管理"));

        var usersPermission = systemGroup.AddPermission(MyPermissions.Users.Default, L("用户管理"));
        usersPermission.AddChild(MyPermissions.Users.Create, L("新增"));
        usersPermission.AddChild(MyPermissions.Users.Update, L("修改"));
        usersPermission.AddChild(MyPermissions.Users.Delete, L("删除"));
    }
    
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PermissonResource>(name);
    }
}