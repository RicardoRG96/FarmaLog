using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class LineaSolicitudTests
    {
        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_LineaHasAQuantityOfZero()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("SKU-1", 0));
        }

        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_LineaHasANegativeQuantity()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("SKU-1", -2));
        }

        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_SkuIsEmpty()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("", 1));
        }

        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_SkuIsWhiteSpace()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("  ", 1));
        }
    }
}
