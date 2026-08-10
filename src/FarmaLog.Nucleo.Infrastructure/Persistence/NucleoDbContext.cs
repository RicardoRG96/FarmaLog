using FarmaLog.Nucleo.Domain.Solicitudes;
using Microsoft.EntityFrameworkCore;

namespace FarmaLog.Nucleo.Infrastructure.Persistence
{
    public sealed class NucleoDbContext(DbContextOptions<NucleoDbContext> options) : DbContext(options)
    {
        public DbSet<SolicitudDeIngresoPedido> Solicitudes => Set<SolicitudDeIngresoPedido>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(NucleoDbContext).Assembly);
    }
}
