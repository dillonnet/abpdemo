using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Volo.Abp.Application.Dtos;

namespace Application.Dto.System.Department;

public class DepartmentListOutput : EntityDto<Guid>
{
    public string Name { get; set; }
    public int Sort { get; set; }
    public DataStatus Status { get; set; }
    public DateTime CreationTime { get; set; }
    public string Remark { get; set; }
    public Guid? ParentId { get; set; }
    public Collection<DepartmentListOutput> Children { get; set; }
}