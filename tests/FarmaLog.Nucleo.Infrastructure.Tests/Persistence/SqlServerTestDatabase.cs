using FarmaLog.Nucleo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Persistence
{
    internal static class SqlServerTestDatabase
    {
        public const string ConnectionString =
        "Server=localhost,1433;Database=FarmaLogTests;User Id=sa;Password=Farmalog!2026;TrustServerCertificate=True";

        public static DbContextOptions<NucleoDbContext> Options() =>
            new DbContextOptionsBuilder<NucleoDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

        public static void Reset()
        {
            using NucleoDbContext context = new(Options());
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }
    }
}
