using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;
using Microsoft.Extensions.Time.Testing;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class SolicitudDeIngresoPedidoTests
    {
        private static SolicitudDeIngresoPedido CrearSolicitud(
            bool esCenabast = false,
            DocumentoVentaCenabast? documentoVentaCenabast = null,
            IReadOnlyCollection<LineaSolicitud>? lineas = null,
            string? observacion = null,
            DateOnly? fechaEntrega = null,
            DateOnly? hoy = null,
            NumeroDelivery? delivery = null,
            string? ordenCompra = null,
            bool urgencia = false)
            => SolicitudDeIngresoPedido.Create(
                esCenabast,
                documentoVentaCenabast,
                lineas ?? CrearLineas(1),
                observacion,
                fechaEntrega,
                hoy ?? new DateOnly(2026, 7, 27),
                delivery,
                ordenCompra,
                urgencia);

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

            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(
                    esCenabast: true, lineas: lineas, fechaEntrega: fechaEntrega, hoy: hoy));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_EsCenabastIsFalseButDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = new[] { LineaSolicitud.Create(
                "SKU-1", 1, "DISPONIBLE", null) };

            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(
                    esCenabast: false, 
                    documentoVentaCenabast: documentoVentaCenabast, 
                    lineas: lineas, 
                    fechaEntrega: fechaEntrega, 
                    hoy: hoy));
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
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = CrearSolicitud(
                esCenabast: true, 
                documentoVentaCenabast: documentoVentaCenabast, 
                lineas: lineas, 
                fechaEntrega: fechaEntrega, 
                hoy: hoy);

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
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = CrearSolicitud(
                esCenabast: false, 
                lineas: lineas, 
                fechaEntrega: fechaEntrega, 
                hoy: hoy);

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
                () => CrearSolicitud(
                    esCenabast: false, 
                    lineas: emptyLineas,
                    fechaEntrega: fechaEntrega, 
                    hoy: hoy));
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
                () => CrearSolicitud(
                    esCenabast: false,
                    lineas: lineas,
                    fechaEntrega: fechaEntrega, 
                    hoy: hoy));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_ThereAre15Lineas()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(15);
            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = CrearSolicitud(
                esCenabast: false, 
                lineas: lineas, 
                fechaEntrega: fechaEntrega, 
                hoy: hoy);

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
                () => CrearSolicitud(
                    esCenabast: false, 
                    lineas: lineas, 
                    observacion: observacion, 
                    fechaEntrega: fechaEntrega, 
                    hoy: hoy));
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
            SolicitudDeIngresoPedido solicitudDeIngresoPedido = CrearSolicitud(
                esCenabast: false,
                lineas: lineas, 
                observacion: observacion, 
                fechaEntrega: fechaEntrega, 
                hoy: hoy);

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
                () => CrearSolicitud(
                    esCenabast: false, 
                    lineas: lineas, 
                    observacion: observacion, 
                    fechaEntrega: fechaEntrega, 
                    hoy: hoy));
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
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(
                esCenabast: false, 
                lineas: lineas, 
                observacion: observacion, 
                fechaEntrega: fechaEntrega, 
                hoy: hoy);

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
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(
                esCenabast: false,
                lineas: lineas, 
                observacion: observacion, 
                fechaEntrega: fechaEntrega, 
                hoy: hoy);

            //Assert
            Assert.AreEqual(fechaEntrega, solicitud.FechaEntregaSolicitada);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldDefaultToToday_When_FechaEntregaSolicitadaIsNull()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(1);
            string observacion = new string('A', 100);
            DateOnly hoy = new DateOnly(2026, 7, 27);
            DateOnly? fechaEntrega = null;

            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(
                esCenabast: false, 
                lineas: lineas, 
                observacion: observacion, 
                fechaEntrega: fechaEntrega, 
                hoy: hoy);

            //Assert
            Assert.AreEqual(hoy, solicitud.FechaEntregaSolicitada);
        }
    }
}
