namespace Application.Permissions;

public class MyPermissions
{
    public const string SystemGroupName = "System";
    
    public static class Departments
    {
        public const string Default = SystemGroupName + ".Departments";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }
    
    public static class Users
    {
        public const string Default = SystemGroupName + ".Users";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }
    
    
    public static class Roles
    {
        public const string Default = SystemGroupName + ".Roles";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }
}