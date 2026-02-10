using ICM_ROTA_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ICM_ROTA_MVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
