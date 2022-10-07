using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Application.Permissions;

public class MyPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myPermissionGroup =  context.AddGroup("权限");
        
        var systemPermission = myPermissionGroup.AddPermission(MyPermissions.SystemGroupName, L(MyPermissions.SystemGroupName));

        
        var departmentPermission = systemPermission.AddChild(MyPermissions.Departments.Default, L(MyPermissions.Departments.Default));
        departmentPermission.AddChild(MyPermissions.Departments.Create, L(MyPermissions.Departments.Create));
        departmentPermission.AddChild(MyPermissions.Departments.Update, L(MyPermissions.Departments.Update));
        departmentPermission.AddChild(MyPermissions.Departments.Delete, L(MyPermissions.Departments.Delete));
        
        var usersPermission = systemPermission.AddChild(MyPermissions.Users.Default, L(MyPermissions.Users.Default));
        usersPermission.AddChild(MyPermissions.Users.Create, L(MyPermissions.Users.Create));
        usersPermission.AddChild(MyPermissions.Users.Update, L(MyPermissions.Users.Update));
        usersPermission.AddChild(MyPermissions.Users.Delete, L(MyPermissions.Users.Delete));
        
        var rolesPermission = systemPermission.AddChild(MyPermissions.Roles.Default, L(MyPermissions.Roles.Default));
        rolesPermission.AddChild(MyPermissions.Roles.Create, L(MyPermissions.Roles.Create));
        rolesPermission.AddChild(MyPermissions.Roles.Update, L(MyPermissions.Roles.Update));
        rolesPermission.AddChild(MyPermissions.Roles.Delete, L(MyPermissions.Roles.Delete));
    }
    
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PermissionResource>(name);
    }
}