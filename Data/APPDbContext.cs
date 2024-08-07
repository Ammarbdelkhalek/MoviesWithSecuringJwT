using JwtWithIdentiyAuthenticatoin.Models.authModels;
using JwtWithIdentiyAuthenticatoin.Models.MoviesModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtWithIdentiyAuthenticatoin.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Movies> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Movies>().HasKey(x => x.MovieId);
            builder.Entity<Movies>().ToTable("movies");
        }
    }
}
