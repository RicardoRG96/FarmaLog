using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Infrastructure.Persistence;
using Microsoft.Extensions.Time.Testing;
using static FarmaLog.Nucleo.Infrastructure.Tests.Persistence.SqlServerTestDatabase;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Persistence
{
    [TestClass]
    [DoNotParallelize]
    public class EfOutboxTests
    {
        [TestInitialize]
        public void Inicializar() => Reset();

        [TestMethod]
        public async Task EfOutbox_Should_PersistPendingEntry_When_MessageIsAddedAndSaved()
        {
            //Arrange
            DateTimeOffset now = new(2026, 10, 14, 19, 0, 0, TimeSpan.Zero);
            FakeTimeProvider clock = new(now);
            await using NucleoDbContext writingContext = new(Options());
            EfOutbox outbox = new(writingContext, clock);
            OutboxMessage message = new(
                Destination: "cola-de-prueba",
                MessageId: "mensaje-1",
                Payload: new PayloadDePrueba("hola"));

            //Act
            outbox.Add(message);
            await writingContext.SaveChangesAsync();

            //Assert
            await using NucleoDbContext readingContext = new(Options());
            OutboxEntry entry = readingContext.Set<OutboxEntry>().Single();

            Assert.AreEqual("cola-de-prueba", entry.Destination);
            Assert.AreEqual("mensaje-1", entry.MessageId);
            Assert.Contains("hola", entry.Payload);
            Assert.AreEqual(now, entry.CreatedAt);
            Assert.IsNull(entry.ProcessedAt);
        }
    }
}
