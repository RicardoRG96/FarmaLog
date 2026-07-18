using FarmaLog.Nucleo.Domain.Solicitudes;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class NumeroDeliveryTests
    {
        [TestMethod]
        public void NumeroDelivery_ShouldThrow_When_AnEmptyStringIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<NumeroDeliveryInvalidoException>(() => NumeroDelivery.Create(""));
        }
    }
}
