using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;

namespace Domain.Data;


[ReplaceDbContext(typeof(IIdentityDbContext))]
public class MyDbContext: AbpDbContext<MyDbContext>, IIdentityDbContext
{
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set;}
    public DbSet<OrganizationUnit> OrganizationUnits { get; set;}
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set;}
    public DbSet<IdentityLinkUser> LinkUsers { get; set;}
    
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ConfigureIdentity();
        builder.ConfigurePermissionManagement();
    }
}