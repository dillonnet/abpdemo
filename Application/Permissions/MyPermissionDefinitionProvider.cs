using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Application.Permissions;

public class MyPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myPermissionGroup =  context.AddGroup("权限");
        
        var systemPermission = myPermissionGroup.AddPermission(MyPermissions.SystemGroupName, L("系统管理"));

        
        var departmentPermission = systemPermission.AddChild(MyPermissions.Departments.Default, L("部门管理"));
        departmentPermission.AddChild(MyPermissions.Departments.Create, L("新增"));
        departmentPermission.AddChild(MyPermissions.Departments.Update, L("修改"));
        departmentPermission.AddChild(MyPermissions.Departments.Delete, L("删除"));
        
        var usersPermission = systemPermission.AddChild(MyPermissions.Users.Default, L("用户管理"));
        usersPermission.AddChild(MyPermissions.Users.Create, L("新增"));
        usersPermission.AddChild(MyPermissions.Users.Update, L("修改"));
        usersPermission.AddChild(MyPermissions.Users.Delete, L("删除"));
        
        var rolesPermission = systemPermission.AddChild(MyPermissions.Roles.Default, L("角色管理"));
        rolesPermission.AddChild(MyPermissions.Roles.Create, L("新增"));
        rolesPermission.AddChild(MyPermissions.Roles.Update, L("修改"));
        rolesPermission.AddChild(MyPermissions.Roles.Delete, L("删除"));
    }
    
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PermissonResource>(name);
    }
}