using Microsoft.EntityFrameworkCore;
using JuntoChallenge.Domain.Entities;

namespace JuntoChallenge.Infrastructure.Repositories
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
