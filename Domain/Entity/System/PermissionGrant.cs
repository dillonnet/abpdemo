using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Domain.Entity.System;

public class PermissionGrant: Volo.Abp.Domain.Entities.Entity<Guid>
{
    [NotNull]
    [MaxLength(32)]
    public  string Name { get; set; }

    [NotNull]
    [MaxLength(32)]
    public  string ProviderName { get; set; }

    [CanBeNull]
    [MaxLength(32)]
    public  string ProviderKey { get;  set; }
}