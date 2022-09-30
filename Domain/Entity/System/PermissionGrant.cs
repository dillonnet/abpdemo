using JetBrains.Annotations;

namespace Domain.Entity.System;

public class PermissionGrant: Volo.Abp.Domain.Entities.Entity<Guid>
{
    [NotNull]
    public  string Name { get; set; }

    [NotNull]
    public  string ProviderName { get; set; }

    [CanBeNull]
    public  string ProviderKey { get;  set; }
}