using FarmaLog.Nucleo.Infrastructure.Messaging;
using FarmaLog.Nucleo.Infrastructure.Persistence;
using Microsoft.Extensions.Time.Testing;
using static FarmaLog.Nucleo.Infrastructure.Tests.Persistence.SqlServerTestDatabase;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Messaging
{
    [TestClass]
    [DoNotParallelize]
    public class OutboxPublisherTests
    {
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
            await using NucleoDbContext firstContext = new(Options());
            firstContext.Add(entry);
            await firstContext.SaveChangesAsync(CancellationToken.None);

            await using NucleoDbContext secondContext = new(Options());
            OutboxMessageSenderSpy spy = new();
            OutboxPublisher publisher = new(secondContext, spy, TimeProvider.System);

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
            await using NucleoDbContext firstContext = new(Options());
            firstContext.Add(entry);
            await firstContext.SaveChangesAsync(CancellationToken.None);
            DateTimeOffset now = new(2026, 10, 17, 10, 0, 0, TimeSpan.Zero);
            FakeTimeProvider clock = new(now);

            await using NucleoDbContext secondContext = new(Options());
            OutboxMessageSenderSpy spy = new();
            OutboxPublisher publisher = new(secondContext, spy, clock);

            //Act
            await publisher.PublishPendingAsync(CancellationToken.None);

            //Assert
            await using NucleoDbContext thirdContext = new(Options());
            OutboxEntry saved = thirdContext.Set<OutboxEntry>().First();
            Assert.AreEqual(now, saved.ProcessedAt);
        }
    }
}
