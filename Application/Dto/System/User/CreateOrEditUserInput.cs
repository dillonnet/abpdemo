using Volo.Abp.Application.Dtos;

namespace Application.Dto.System.User;

public class CreateOrEditUserInput
{
    public string UserName { get; set; }
    
    public string Name { get; set; }
    
    public string Password { get; set; }
    
    public Guid[] RoleIds { get; set; }
    
    public Guid? DepartmentId { get; set; }
}