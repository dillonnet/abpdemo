namespace Application.Dto.System.Role;

public class RoleDetailOutput: RoleListOutput
{
    public string[] Permissions { get; set; }
}