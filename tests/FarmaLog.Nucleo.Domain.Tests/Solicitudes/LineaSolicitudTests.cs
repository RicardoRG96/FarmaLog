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
                () => LineaSolicitud.Create("SKU-1", 0, "DISPONIBLE", null));
        }

        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_LineaHasANegativeQuantity()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("SKU-1", -2, "DISPONIBLE", null));
        }

        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_SkuIsEmpty()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("", 1, "DISPONIBLE", null));
        }

        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_SkuIsWhiteSpace()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("  ", 1, "DISPONIBLE", null));
        }

        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_SkuIsNull()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create(null, 1, "DISPONIBLE", null));
        }

        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_EstadoInventarioIsEmpty()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("SKU-1", 1, "", null));
        }

        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_EstadoInventarioIsWhiteSpace()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("SKU-1", 1, "  ", null));
        }

        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_EstadoInventarioIsNull()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("SKU-1", 1, null, null));
        }
    }
}
