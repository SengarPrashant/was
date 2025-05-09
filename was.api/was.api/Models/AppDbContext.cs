using Microsoft.EntityFrameworkCore;
using was.api.Models.Dtos;

namespace was.api.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DtoRoles> Roles { get; set; }
        public DbSet<DtoUser> Users { get; set; }

    }
}
