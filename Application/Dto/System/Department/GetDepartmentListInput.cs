using Domain.Enums;
using Volo.Abp.Application.Dtos;

namespace Application.Dto.System.Department;

public class GetDepartmentListInput: PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    
    public DataStatus? Status { get; set; }
}