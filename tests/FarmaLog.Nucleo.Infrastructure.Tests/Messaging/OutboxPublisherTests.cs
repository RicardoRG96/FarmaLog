using FarmaLog.Nucleo.Infrastructure.Messaging;
using FarmaLog.Nucleo.Infrastructure.Persistence;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using static FarmaLog.Nucleo.Infrastructure.Tests.Persistence.SqlServerTestDatabase;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Messaging
{
    [TestClass]
    [DoNotParallelize]
    public class OutboxPublisherTests
    {
        private static async Task InsertEntryAsync(OutboxEntry entry)
        {
            await using NucleoDbContext context = new(Options());
            context.Add(entry);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        [TestInitialize]
        public void Inicializar() => Reset();

        [TestMethod]
        public async Task OutboxPublisher_Should_SendEntryWithItsDestinationMessageIdAndPayload_When_EntryIsPending()
        {
            //Arrange
            OutboxEntry entry = new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "cola-de-prueba",
                "message-id-prueba",
                "payload-de-prueba",
                new DateTimeOffset(2026, 10, 16, 12, 0, 0, TimeSpan.Zero));
            await InsertEntryAsync(entry);

            await using NucleoDbContext secondContext = new(Options());
            OutboxMessageSenderSpy spy = new();
            OutboxPublisher publisher = new(
                secondContext, spy, TimeProvider.System, NullLogger<OutboxPublisher>.Instance);

            //Act
            await publisher.PublishPendingAsync(CancellationToken.None);

            //Assert
            SentMessage sent = Assert.ContainsSingle(spy.Sent);
            Assert.AreEqual("cola-de-prueba", sent.Destination);
            Assert.AreEqual("message-id-prueba", sent.MessageId);
            Assert.AreEqual("payload-de-prueba", sent.Payload);
        }

        [TestMethod]
        public async Task OutboxPublisher_Should_MarkEntryAsProcessedAtCurrentTime_When_EntryIsSent()
        {
            //Arrange
            OutboxEntry entry = new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "cola-de-prueba",
                "message-id-prueba",
                "payload-de-prueba",
                new DateTimeOffset(2026, 10, 16, 12, 0, 0, TimeSpan.Zero));
            await InsertEntryAsync(entry);

            DateTimeOffset now = new(2026, 10, 17, 10, 0, 0, TimeSpan.Zero);
            FakeTimeProvider clock = new(now);

            await using NucleoDbContext secondContext = new(Options());
            OutboxMessageSenderSpy spy = new();
            OutboxPublisher publisher = new(
                secondContext, spy, clock, NullLogger<OutboxPublisher>.Instance);

            //Act
            await publisher.PublishPendingAsync(CancellationToken.None);

            //Assert
            await using NucleoDbContext thirdContext = new(Options());
            OutboxEntry saved = thirdContext.Set<OutboxEntry>().Single();
            Assert.AreEqual(now, saved.ProcessedAt);
        }

        [TestMethod]
        public async Task OutboxPublisher_Should_SendOnlyPendingEntries_When_SomeEntriesWereAlreadyProcessed()
        {
            //Arrange
            OutboxEntry firstEntry = new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "cola-de-prueba",
                "message-pendiente",
                "payload-de-prueba",
                new DateTimeOffset(2026, 10, 16, 12, 0, 0, TimeSpan.Zero));

            OutboxEntry secondEntry = new(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "cola-de-prueba",
                "message-procesado",
                "payload-de-prueba2",
                new DateTimeOffset(2026, 10, 16, 12, 0, 0, TimeSpan.Zero));

            DateTimeOffset now = new(2026, 10, 17, 10, 0, 0, TimeSpan.Zero);
            FakeTimeProvider clock = new(now);

            await InsertEntryAsync(firstEntry);
            secondEntry.MarkAsProcessed(clock.GetUtcNow());
            await InsertEntryAsync(secondEntry);

            await using NucleoDbContext secondContext = new(Options());
            OutboxMessageSenderSpy spy = new();
            OutboxPublisher publisher = new(
                secondContext, spy, clock, NullLogger<OutboxPublisher>.Instance);

            //Act
            await publisher.PublishPendingAsync(CancellationToken.None);

            //Assert
            SentMessage sent = Assert.ContainsSingle(spy.Sent);
            Assert.AreEqual("message-pendiente", sent.MessageId);
        }

        [TestMethod]
        public async Task OutboxPublisher_Should_KeepFailedEntryPendingAndProcessTheOthers_When_OneSendFails()
        {
            //Arrange
            OutboxEntry firstEntry = new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "cola-de-prueba",
                "mensaje-fallido",
                "payload-de-prueba",
                new DateTimeOffset(2026, 10, 16, 12, 0, 0, TimeSpan.Zero));

            OutboxEntry secondEntry = new(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "cola-de-prueba",
                "mensaje-exitoso",
                "payload-de-prueba2",
                new DateTimeOffset(2026, 10, 16, 13, 0, 0, TimeSpan.Zero));

            await InsertEntryAsync(firstEntry);
            await InsertEntryAsync(secondEntry);

            DateTimeOffset now = new(2026, 10, 17, 10, 0, 0, TimeSpan.Zero);
            FakeTimeProvider clock = new(now);

            await using NucleoDbContext secondContext = new(Options());
            OutboxMessageSenderSpy spy = new("mensaje-fallido");
            OutboxPublisher publisher = new(
                secondContext, spy, clock, NullLogger<OutboxPublisher>.Instance);

            //Act
            await publisher.PublishPendingAsync(CancellationToken.None);

            //Assert
            await using NucleoDbContext thirdContext = new(Options());
            OutboxEntry failedEntry = thirdContext
                .Set<OutboxEntry>()
                .Where(e => e.MessageId == "mensaje-fallido")
                .Single();

            OutboxEntry successEntry = thirdContext
                .Set<OutboxEntry>()
                .Where(e => e.MessageId == "mensaje-exitoso")
                .Single();

            Assert.IsNull(failedEntry.ProcessedAt);
            Assert.AreEqual(now, successEntry.ProcessedAt);
        }
    }
}
