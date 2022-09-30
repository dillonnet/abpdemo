namespace Application.Dto.System.User;

public class UserInfoOutput: UserListOutput
{
    public string Permissions { get; set; }
    
    public string Menus { get; set; }
}