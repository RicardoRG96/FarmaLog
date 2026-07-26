using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class SolicitudDeIngresoPedidoTests
    {
        private static IReadOnlyCollection<LineaSolicitud> CrearLineas(int cantidad) =>
            Enumerable.Range(1, cantidad)
                .Select(i => LineaSolicitud.Create($"SKU-{i}", 1, "DISPONIBLE", null))
                .ToArray();

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_EsCenabastIsTrueButDocumentoVentaCenabastIsNull()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", 1, "DISPONIBLE", null) };

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(true, null, lineas, null));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_EsCenabastIsFalseButDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", 1, "DISPONIBLE", null) };

            //Arrange
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(false, documentoVentaCenabast, lineas, null));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_EsCenabastIsTrueAndDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");
            
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", 1, "DISPONIBLE", null) };

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                true, documentoVentaCenabast, lineas, null);

            //Assert
            Assert.AreSame(documentoVentaCenabast, solicitudDeIngresoPedido.DocumentoVentaCenabast);
            Assert.IsTrue(solicitudDeIngresoPedido.EsCenabast);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_EsCenabastIsFalseAndDocumentoVentaCenabastIsNull()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", 1, "DISPONIBLE", null) };

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                false, null, lineas, null);

            //Assert
            Assert.IsNull(solicitudDeIngresoPedido.DocumentoVentaCenabast);
            Assert.IsFalse(solicitudDeIngresoPedido.EsCenabast);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_LineasAreEmpty()
        {
            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(false, null, Array.Empty<LineaSolicitud>(), null));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_ThereAreMoreThan15Lineas()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(16);

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(false, null, lineas, null));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_ThereAre15Lineas()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(15);

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                false, null, lineas, null);

            //Assert
            Assert.IsNotNull(solicitudDeIngresoPedido);
            Assert.HasCount(15, solicitudDeIngresoPedido.Lineas);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_ObservacionHasMoreThan300Characters()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(1);
            string observacion = new string('A', 301);

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(false, null, lineas, observacion));
        }
    }
}
