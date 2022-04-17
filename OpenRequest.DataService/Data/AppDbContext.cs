using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.Data;

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
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
}