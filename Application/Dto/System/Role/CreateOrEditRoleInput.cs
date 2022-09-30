using Volo.Abp.Application.Dtos;

namespace Application.Dto.System.Role;

public class CreateOrEditRoleInput: EntityDto<Guid?>
{
    public string Name { get; set; }
    
    public string Remark { get; set; }
    
    public string[] Permissions { get; set; }
}