using Microsoft.EntityFrameworkCore;
using OrganisationalAuth.Entities;

namespace OrganisationalAuth.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}
