using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenRequest.Core.Entities;

namespace OpenRequest.Infrastructure.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Freelancer> Freelancers { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<PostCategory> PostCategory { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<FreelancerRequest> FreelancerRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}