using Microsoft.EntityFrameworkCore;

namespace PizzaAPI.Models
{
    public partial class pizzariaContext : DbContext
    {
        public pizzariaContext()
        {
        }

        public pizzariaContext(DbContextOptions<pizzariaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Pedidos> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
