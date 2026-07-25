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
            //Act + Assert
            Assert.ThrowsExactly<SolicitudIncoherenteRespectoACenabastException>(
                () => SolicitudDeIngresoPedido.Create(true, null));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_EsCenabastIsFalseButDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");

            //Act + Assert
            Assert.ThrowsExactly<SolicitudIncoherenteRespectoACenabastException>(
                () => SolicitudDeIngresoPedido.Create(false, documentoVentaCenabast));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_EsCenabastIsTrueAndDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            string expected = "123456789";
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                true, documentoVentaCenabast);

            //Assert
            Assert.AreEqual(expected, solicitudDeIngresoPedido.DocumentoVentaCenabast!.Documento);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_EsCenabastIsFalseAndDocumentoVentaCenabastIsNull()
        {
            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                false, null);

            //Assert
            Assert.IsNotNull(solicitudDeIngresoPedido);
        }
    }
}
