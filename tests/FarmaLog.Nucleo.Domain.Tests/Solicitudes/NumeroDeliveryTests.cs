using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

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

        [TestMethod]
        public void NumeroDelivery_ShouldThrow_When_NullIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<NumeroDeliveryInvalidoException>(() => NumeroDelivery.Create(null));
        }
    }
}
