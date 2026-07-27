using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;
using Microsoft.Extensions.Time.Testing;

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

            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(
                    true, null, lineas, null, fechaEntrega, hoy));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_EsCenabastIsFalseButDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", 1, "DISPONIBLE", null) };

            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Arrange
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(
                    false, documentoVentaCenabast, lineas, null, fechaEntrega, hoy));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_EsCenabastIsTrueAndDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");
            
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", 1, "DISPONIBLE", null) };

            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                true, documentoVentaCenabast, lineas, null, fechaEntrega, hoy);

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

            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                false, null, lineas, null, fechaEntrega, hoy);

            //Assert
            Assert.IsNull(solicitudDeIngresoPedido.DocumentoVentaCenabast);
            Assert.IsFalse(solicitudDeIngresoPedido.EsCenabast);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_LineasAreEmpty()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> emptyLineas = Array.Empty<LineaSolicitud>();
            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(
                    false, null, emptyLineas, null, fechaEntrega, hoy));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_ThereAreMoreThan15Lineas()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(16);
            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(
                    false, null, lineas, null, fechaEntrega, hoy));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_ThereAre15Lineas()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(15);
            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                false, null, lineas, null, fechaEntrega, hoy);

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
            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(
                    false, null, lineas, observacion, fechaEntrega, hoy));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_ObservacionHas300Characters()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(1);
            string observacion = new string('A', 300);
            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = SolicitudDeIngresoPedido.Create(
                false, null, lineas, observacion, fechaEntrega, hoy);

            //Assert
            Assert.IsNotNull(solicitudDeIngresoPedido);
            Assert.IsNotNull(solicitudDeIngresoPedido.Observacion);
            Assert.HasCount(300, solicitudDeIngresoPedido.Observacion);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_FechaEntregaSolicitadaIsEarlierThanToday()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(1);
            string observacion = new string('A', 100);
            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 25);

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => SolicitudDeIngresoPedido.Create(
                    false, null, lineas, observacion, fechaEntrega, hoy));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_FechaEntregaSolicitadaIsToday()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(1);
            string observacion = new string('A', 100);
            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 27);

            //Act
            SolicitudDeIngresoPedido solicitud = SolicitudDeIngresoPedido.Create(
                false, null, lineas, observacion, fechaEntrega, hoy);

            //Assert
            Assert.AreEqual(fechaEntrega, solicitud.FechaEntregaSolicitada);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_FechaEntregaSolicitadaIsInTheFuture()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(1);
            string observacion = new string('A', 100);
            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act
            SolicitudDeIngresoPedido solicitud = SolicitudDeIngresoPedido.Create(
                false, null, lineas, observacion, fechaEntrega, hoy);

            //Assert
            Assert.AreEqual(fechaEntrega, solicitud.FechaEntregaSolicitada);
        }
    }
}
