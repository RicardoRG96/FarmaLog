using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class SolicitudDeIngresoPedidoTests
    {
        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_EsCenabastIsTrueButDocumentoVentaCenabastIsNull()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", "DISPONIBLE", 1, null) };

            //Act + Assert
            Assert.ThrowsExactly<SolicitudIncoherenteRespectoACenabastException>(
                () => SolicitudDeIngresoPedido.Create(true, null, lineas));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_EsCenabastIsFalseButDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", "DISPONIBLE", 1, null) };

            //Arrange
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");

            //Act + Assert
            Assert.ThrowsExactly<SolicitudIncoherenteRespectoACenabastException>(
                () => SolicitudDeIngresoPedido.Create(false, documentoVentaCenabast, lineas));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_EsCenabastIsTrueAndDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");
            
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", "DISPONIBLE", 1, null) };

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                true, documentoVentaCenabast, lineas);

            //Assert
            Assert.AreSame(documentoVentaCenabast, solicitudDeIngresoPedido.DocumentoVentaCenabast);
            Assert.IsTrue(solicitudDeIngresoPedido.EsCenabast);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_EsCenabastIsFalseAndDocumentoVentaCenabastIsNull()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", "DISPONIBLE", 1, null) };

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                false, null, lineas);

            //Assert
            Assert.IsNull(solicitudDeIngresoPedido.DocumentoVentaCenabast);
            Assert.IsFalse(solicitudDeIngresoPedido.EsCenabast);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_LineasAreEmpty()
        {
            //Act + Assert
            Assert.ThrowsExactly<SolicitudConLineasInvalidasException>(
                () => SolicitudDeIngresoPedido.Create(false, null, Array.Empty<LineaSolicitud>()));
        }
    }
}
