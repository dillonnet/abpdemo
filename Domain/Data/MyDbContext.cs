using Domain.Entity.System;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Domain.Data;


public class MyDbContext: AbpDbContext<MyDbContext>
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>().ConfigureByConvention();
        builder.Entity<Role>().ConfigureByConvention();
        builder.Entity<UserRole>(ur =>
        {
            ur.HasKey(c => new { c.RoleId, c.UserId });
        });
        
        builder.Entity<Department>().ConfigureByConvention();
        builder.Entity<PermissionGrant>();
    }
}