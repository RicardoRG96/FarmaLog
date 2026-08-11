using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Infrastructure.Clock;
using Microsoft.Extensions.Time.Testing;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Clock
{
    [TestClass]
    public class RelojDelSistemaTests
    {
        [TestMethod]
        public void RelojDelSistema_ShouldReturnThePreviousDay_When_UtcCrossedMidnightButSantiagoDidNot()
        {
            //Arrange
            FakeTimeProvider tiempo = new FakeTimeProvider(
                new DateTimeOffset(2026, 8, 2, 2, 30, 0, TimeSpan.Zero));

            IReloj reloj = new RelojDelSistema(tiempo);

            //Act
            var hoy = reloj.Hoy;

            //Assert
            Assert.AreEqual(new DateOnly(2026, 8, 1), hoy);
        }
    }
}
