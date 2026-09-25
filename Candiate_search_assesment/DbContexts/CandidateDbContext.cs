using Candiate_search_assesment.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Candiate_search_assesment.DbContexts
{
    public class CandidateDbContext : DbContext
    {
        public CandidateDbContext(DbContextOptions<CandidateDbContext> options) : base(options)
        {
        }

        public DbSet<Candidates> Candidates => Set<Candidates>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Candidates>(entity =>
            {
                entity.HasIndex(c => c.Email).IsUnique();
            });
        }
    }
}
