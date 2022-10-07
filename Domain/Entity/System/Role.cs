using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Domain.Entity.System;

public class Role: FullAuditedAggregateRoot<Guid>
{
    [MaxLength(32)]
    public string Name { get; set; }
    
    public bool IsStatic { get; set; }
    
    [MaxLength(256)]
    public string Remark { get; set; }
}