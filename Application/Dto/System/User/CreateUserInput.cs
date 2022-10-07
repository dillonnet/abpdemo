using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Application.Dtos;

namespace Application.Dto.System.User;


public class EditUserInput
{
    [StringLength(maximumLength: Domain.Entity.System.User.MAX_LENGTH_USER_NAME,MinimumLength = 6)]
    [DisplayName("用户名")]
    [Required]
    public string UserName { get; set; }
    
    [StringLength(Domain.Entity.System.User.MAX_LENGTH_NAME)]
    [Required]
    public string Name { get; set; }

    public Guid[] RoleIds { get; set; }
    
    public Guid? DepartmentId { get; set; }
}

public class CreateUserInput: EditUserInput
{
    [StringLength(18,MinimumLength = 6)]
    [DisplayName("密码")]
    [Required]
    public string Password { get; set; }
}