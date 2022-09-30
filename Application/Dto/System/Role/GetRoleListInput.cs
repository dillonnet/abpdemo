using Volo.Abp.Application.Dtos;

namespace Application.Dto.System.Role;

public class GetRoleListInput: PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}