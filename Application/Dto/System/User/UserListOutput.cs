﻿using Volo.Abp.Application.Dtos;

namespace Application.Dto.System.User;

public class UserListOutput: EntityDto<Guid>
{
    public string UserName { get; set; }
    
    public string Name { get; set; }
    
    public Guid[] RoleIds { get; set; }
}