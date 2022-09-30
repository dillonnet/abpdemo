using Volo.Abp.Application.Dtos;

namespace Application.Dto.System.Role;

public class RoleListOutput: EntityDto<Guid>
{
    public string Name { get; set; }
    public bool IsStatic { get; set; }
    public string Remark { get; set; }
}