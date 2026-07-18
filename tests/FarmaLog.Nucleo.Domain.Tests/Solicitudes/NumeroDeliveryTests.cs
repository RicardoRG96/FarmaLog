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

        [TestMethod]
        public void NumeroDelivery_ShouldThrow_When_AWhiteSpaceStringIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<NumeroDeliveryInvalidoException>(() => NumeroDelivery.Create("   "));
        }

        [TestMethod]
        public void NumeroDelivery_Should_ConstructANumeroDeliveryWithoutWhiteSpaces_When_AStringWithWhiteSpacesIsPassed()
        {
            //Arrange
            string expected = "123";

            //Act
            NumeroDelivery numeroDelivery = NumeroDelivery.Create("  123  ");

            //Assert
            Assert.AreEqual(expected, numeroDelivery.Numero);
        }
    }
}
