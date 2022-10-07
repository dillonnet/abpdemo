using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dto.System.Department;

public class CreateOrEditDepartmentInput
{
    [Required]
    [StringLength(maximumLength: 32)]
    public string Name { get; set; }
    public int Sort { get; set; }
    public DataStatus Status { get; set; }
    public Guid? ParentId { get; set; }
    [StringLength(maximumLength: 256)]
    public string Remark { get; set; }
}