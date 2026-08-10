using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;
using Microsoft.Extensions.Time.Testing;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class SolicitudDeIngresoPedidoTests
    {
        public static SolicitudDeIngresoPedido CrearSolicitud(
            Guid? id = null,
            CodigoLaboratorio codigoLaboratorio = null,
            CuentaCliente cuentaCliente = null,
            DireccionDespacho direccionDespacho = null,
            TipoOrdenVenta tipoOrdenVenta = null,
            bool esCenabast = false,
            DocumentoVentaCenabast? documentoVentaCenabast = null,
            IReadOnlyCollection<LineaSolicitud>? lineas = null,
            string? observacion = null,
            DateOnly? fechaEntrega = null,
            DateOnly? hoy = null,
            NumeroDelivery? numeroDelivery = null,
            string? ordenCompra = null,
            bool urgencia = false)
            => SolicitudDeIngresoPedido.Create(
                id ?? Guid.Parse("11111111-1111-1111-1111-111111111111"),
                codigoLaboratorio ?? CodigoLaboratorio.Create("23"),
                cuentaCliente ?? CuentaCliente.Create("23-0778903671"),
                direccionDespacho ?? DireccionDespacho.Create("23-778903671D1"),
                tipoOrdenVenta ?? TipoOrdenVenta.Create("23F1"),
                esCenabast,
                documentoVentaCenabast,
                lineas ?? CrearLineas(1),
                observacion,
                fechaEntrega,
                hoy ?? new DateOnly(2026, 7, 27),
                numeroDelivery ?? NumeroDelivery.Create("123456789"),
                ordenCompra,
                urgencia);

        private static SolicitudDeIngresoPedido RehidratarSolicitud(
            EstadoSolicitud estado,
            IReadOnlyCollection<string>? motivos = null,
            Guid? id = null)
            => SolicitudDeIngresoPedido.Rehidratar(
                id ?? Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CodigoLaboratorio.Create("23"),
                CuentaCliente.Create("23-0778903671"),
                DireccionDespacho.Create("23-778903671D1"),
                TipoOrdenVenta.Create("23F1"),
                false,
                null,
                CrearLineas(1),
                null,
                new DateOnly(2026, 7, 27),
                NumeroDelivery.Create("123456789"),
                null,
                false,
                estado,
                motivos ?? []);

        private static IReadOnlyCollection<LineaSolicitud> CrearLineas(int cantidad) =>
            Enumerable.Range(1, cantidad)
                .Select(i => LineaSolicitud.Create($"SKU-{i}", 1, "DISPONIBLE", null))
                .ToArray();

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_EsCenabastIsTrueButDocumentoVentaCenabastIsNull()
        {
            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(esCenabast: true));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_EsCenabastIsFalseButDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(documentoVentaCenabast: documentoVentaCenabast));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_EsCenabastIsTrueAndDocumentoVentaCenabastIsPresent()
        {
            //Arrange
            DocumentoVentaCenabast documentoVentaCenabast = DocumentoVentaCenabast.Create("123456789");
            
            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(
                esCenabast: true, 
                documentoVentaCenabast: documentoVentaCenabast);

            //Assert
            Assert.AreSame(documentoVentaCenabast, solicitud.DocumentoVentaCenabast);
            Assert.IsTrue(solicitud.EsCenabast);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_EsCenabastIsFalseAndDocumentoVentaCenabastIsNull()
        {
            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();

            //Assert
            Assert.IsNull(solicitud.DocumentoVentaCenabast);
            Assert.IsFalse(solicitud.EsCenabast);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_LineasAreEmpty()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> emptyLineas = Array.Empty<LineaSolicitud>();

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(lineas: emptyLineas));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_ThereAreMoreThan15Lineas()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(16);

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(lineas: lineas));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_ThereAre15Lineas()
        {
            //Arrange
            IReadOnlyCollection<LineaSolicitud> lineas = CrearLineas(15);

            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(lineas: lineas);

            //Assert
            Assert.IsNotNull(solicitud);
            Assert.HasCount(15, solicitud.Lineas);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_ObservacionHasMoreThan300Characters()
        {
            //Arrange
            string observacion = new('A', 301);

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(observacion: observacion));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_ObservacionHas300Characters()
        {
            //Arrange
            string observacion = new string('A', 300);

            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(observacion: observacion);

            //Assert
            Assert.IsNotNull(solicitud);
            Assert.IsNotNull(solicitud.Observacion);
            Assert.HasCount(300, solicitud.Observacion);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_FechaEntregaSolicitadaIsEarlierThanToday()
        {
            //Arrange
            DateOnly fechaEntrega = new DateOnly(2026, 7, 25);

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(fechaEntrega: fechaEntrega));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_FechaEntregaSolicitadaIsToday()
        {
            //Arrange
            DateOnly fechaEntrega = new DateOnly(2026, 7, 27);

            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(fechaEntrega: fechaEntrega);

            //Assert
            Assert.AreEqual(fechaEntrega, solicitud.FechaEntregaSolicitada);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_FechaEntregaSolicitadaIsInTheFuture()
        {
            //Arrange
            DateOnly fechaEntrega = new DateOnly(2026, 7, 28);

            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(fechaEntrega: fechaEntrega);

            //Assert
            Assert.AreEqual(fechaEntrega, solicitud.FechaEntregaSolicitada);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldDefaultToToday_When_FechaEntregaSolicitadaIsNull()
        {
            //Arrange
            DateOnly hoy = new DateOnly(2026, 7, 27);

            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(hoy: hoy);

            //Assert
            Assert.AreEqual(hoy, solicitud.FechaEntregaSolicitada);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_NumeroDeliveryIsPresent()
        {
            //Arrange
            NumeroDelivery numeroDelivery = NumeroDelivery.Create("123456789");

            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(numeroDelivery: numeroDelivery);

            //Assert
            Assert.AreSame(numeroDelivery, solicitud.NumeroDelivery);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_OrdenCompraIsPresent()
        {
            //Arrange
            string ordenCompra = "123456789";

            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(ordenCompra: ordenCompra);

            //Assert
            Assert.AreEqual(ordenCompra, solicitud.OrdenCompra);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_OrdenCompraIsNull()
        {
            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();

            //Assert
            Assert.IsNull(solicitud.OrdenCompra);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_When_UrgenciaIsTrue()
        {
            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(urgencia: true);

            //Assert
            Assert.IsTrue(solicitud.Urgencia);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_NumeroDeliveryIsNull()
        {
            //Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => SolicitudDeIngresoPedido.Create(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CodigoLaboratorio.Create("23"),
                    CuentaCliente.Create("23-0778903671"),
                    DireccionDespacho.Create("23-778903671D1"),
                    TipoOrdenVenta.Create("23F1"),
                    false,
                    null,
                    CrearLineas(1),
                    null,
                    null,
                    new DateOnly(2026, 7, 27),
                    null,
                    null,
                    false));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_LineasIsNull()
        {
            //Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => SolicitudDeIngresoPedido.Create(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CodigoLaboratorio.Create("23"),
                    CuentaCliente.Create("23-0778903671"),
                    DireccionDespacho.Create("23-778903671D1"),
                    TipoOrdenVenta.Create("23F1"),
                    false,
                    null,
                    null,
                    null,
                    null,
                    new DateOnly(2026, 7, 27),
                    NumeroDelivery.Create("123456789"),
                    null,
                    false));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_CodigoLaboratorioIsNull()
        {
            //Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => SolicitudDeIngresoPedido.Create(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    null,
                    CuentaCliente.Create("23-0778903671"),
                    DireccionDespacho.Create("23-778903671D1"),
                    TipoOrdenVenta.Create("23F1"),
                    false,
                    null,
                    CrearLineas(1),
                    null,
                    null,
                    new DateOnly(2026, 7, 27),
                    NumeroDelivery.Create("123456789"),
                    null,
                    false));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_CuentaClienteIsNull()
        {
            //Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => SolicitudDeIngresoPedido.Create(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CodigoLaboratorio.Create("23"),
                    null,
                    DireccionDespacho.Create("23-778903671D1"),
                    TipoOrdenVenta.Create("23F1"),
                    false,
                    null,
                    CrearLineas(1),
                    null,
                    null,
                    new DateOnly(2026, 7, 27),
                    NumeroDelivery.Create("123456789"),
                    null,
                    false));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_DireccionDespachoIsNull()
        {
            //Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => SolicitudDeIngresoPedido.Create(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CodigoLaboratorio.Create("23"),
                    CuentaCliente.Create("23-0778903671"),
                    null,
                    TipoOrdenVenta.Create("23F1"),
                    false,
                    null,
                    CrearLineas(1),
                    null,
                    null,
                    new DateOnly(2026, 7, 27),
                    NumeroDelivery.Create("123456789"),
                    null,
                    false));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_TipoOrdenVentaIsNull()
        {
            //Act + Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => SolicitudDeIngresoPedido.Create(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CodigoLaboratorio.Create("23"),
                    CuentaCliente.Create("23-0778903671"),
                    DireccionDespacho.Create("23-778903671D1"),
                    null,
                    false,
                    null,
                    CrearLineas(1),
                    null,
                    null,
                    new DateOnly(2026, 7, 27),
                    NumeroDelivery.Create("123456789"),
                    null,
                    false));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_CuentaClientePrefixIsDifferentFromCodigoLaboratorio()
        {
            //Arrange
            CuentaCliente cuentaCliente = CuentaCliente.Create("41-0778903671");

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(cuentaCliente: cuentaCliente));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_DireccionDespachoPrefixIsDifferentFromCodigoLaboratorio()
        {
            //Arrange
            DireccionDespacho direccionDespacho = DireccionDespacho.Create("41-778903671D1");

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(direccionDespacho: direccionDespacho));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_TipoOrdenVentaPrefixIsDifferentFromCodigoLaboratorio()
        {
            //Arrange
            TipoOrdenVenta tipoOrdenVenta = TipoOrdenVenta.Create("41F1");

            //Act + Assert
            Assert.ThrowsExactly<SolicitudInvalidaException>(
                () => CrearSolicitud(tipoOrdenVenta: tipoOrdenVenta));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldNotBeAffected_When_LinesAreMutatedAfterConstruction()
        {
            //Arrange
            List<LineaSolicitud> lineas = Enumerable.Range(1, 15)
                .Select(i => LineaSolicitud.Create($"SKU-{i}", 1, "DISPONIBLE", null))
                .ToList();

            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(lineas: lineas);
            lineas.Add(LineaSolicitud.Create($"SKU-16", 1, "DISPONIBLE", null));

            //Assert
            Assert.HasCount(15, solicitud.Lineas);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_WithEstadoRecibida()
        {
            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();

            //Assert
            Assert.AreEqual(EstadoSolicitud.Recibida, solicitud.Estado);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldChangeEstadoToAceptada_When_AceptarIsCalled()
        {
            //Arrange
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();

            //Act
            solicitud.Aceptar();

            //Assert
            Assert.AreEqual(EstadoSolicitud.Aceptada, solicitud.Estado);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldChangeEstadoToRechazada_When_RechazarIsCalled()
        {
            //Arrange
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();
            IReadOnlyCollection<string> motivos = ["Stock insuficiente de SKU-1"];

            //Act
            solicitud.Rechazar(motivos);

            //Assert
            Assert.AreEqual(EstadoSolicitud.Rechazada, solicitud.Estado);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldRecordMotivosDeRechazo_When_RechazarIsCalled()
        {
            //Arrange
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();
            IReadOnlyCollection<string> motivos =
            [
                "Stock insuficiente de SKU-1",
                "Cuenta de cliente bloqueada"
            ];

            //Act
            solicitud.Rechazar(motivos);

            //Assert
            CollectionAssert.AreEqual(motivos.ToList(), solicitud.MotivosDeRechazo.ToList());
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstruct_WithoutMotivosDeRechazo()
        {
            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();

            //Assert
            Assert.IsEmpty(solicitud.MotivosDeRechazo);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldNotBeAffected_When_MotivosDeRechazoAreMutatedAfterRechazar()
        {
            //Arrange
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();
            List<string> motivos =
            [
                "Stock insuficiente de SKU-1",
                "Cliente bloqueado"
            ];

            //Act
            solicitud.Rechazar(motivos);
            motivos.Add("El lote 12345 no existe");

            //Assert
            Assert.HasCount(2, solicitud.MotivosDeRechazo);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldKeepEstadoAceptada_When_AceptarIsCalledMoreThanOnce()
        {
            //Arrange
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();
            solicitud.Aceptar();

            //Act
            solicitud.Aceptar();

            //Assert
            Assert.AreEqual(EstadoSolicitud.Aceptada, solicitud.Estado);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldKeepEstadoRechazada_When_RechazarIsCalledMoreThanOnce()
        {
            //Arrange
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();
            IReadOnlyCollection<string> motivos =
            [
                "Stock insuficiente de SKU-1",
                "Cuenta de cliente bloqueada"
            ];
            solicitud.Rechazar(motivos);

            //Act
            solicitud.Rechazar(motivos);

            //Assert
            Assert.AreEqual(EstadoSolicitud.Rechazada, solicitud.Estado);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldKeepMotivosDeRechazo_When_RechazarIsCalledAgainWithDifferentMotivos()
        {
            //Arrange
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();
            IReadOnlyCollection<string> primerosMotivos =
            [
                "Stock insuficiente de SKU-1",
                "Cuenta de cliente bloqueada"
            ];
            solicitud.Rechazar(primerosMotivos);

            IReadOnlyCollection<string> segundosMotivos =
            [
                "El lote 12345, no existe",
                "Dirección de despacho no existe"
            ];

            //Act
            solicitud.Rechazar(segundosMotivos);

            //Assert
            CollectionAssert.AreEqual(primerosMotivos.ToList(), solicitud.MotivosDeRechazo.ToList());
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_AceptarIsCalledAndSolicitudHasEstadoRechazada()
        {
            //Arrange
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();
            IReadOnlyCollection<string> motivos =
            [
                "Stock insuficiente de SKU-1",
                "Cuenta de cliente bloqueada"
            ];
            solicitud.Rechazar(motivos);

            //Act +Assert
            Assert.ThrowsExactly<SolicitudYaResueltaException>(
                () => solicitud.Aceptar());
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_RechazarIsCalledAndSolicitudHasEstadoAceptada()
        {
            //Arrange
            SolicitudDeIngresoPedido solicitud = CrearSolicitud();
            solicitud.Aceptar();
            IReadOnlyCollection<string> motivos =
            [
                "Stock insuficiente de SKU-1",
                "Cuenta de cliente bloqueada"
            ];

            //Act +Assert
            Assert.ThrowsExactly<SolicitudYaResueltaException>(
                () => solicitud.Rechazar(motivos));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldConstructWithId_When_IdIsPassed()
        {
            //Arrange
            Guid expected = Guid.Parse("11111111-1111-1111-1111-111111111111");

            //Act
            SolicitudDeIngresoPedido solicitud = CrearSolicitud(id: expected);

            //Assert
            Assert.AreEqual(expected, solicitud.Id);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldThrow_When_AnEmptyIdIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => SolicitudDeIngresoPedido.Create(
                    Guid.Empty,
                    CodigoLaboratorio.Create("23"),
                    CuentaCliente.Create("23-0778903671"),
                    DireccionDespacho.Create("23-778903671D1"),
                    TipoOrdenVenta.Create("23F1"),
                    false,
                    null,
                    CrearLineas(1),
                    null,
                    null,
                    new DateOnly(2026, 7, 27),
                    NumeroDelivery.Create("123456789"),
                    null,
                    false));
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldPreserveEstado_When_IsRehidratada()
        {
            //Act
            SolicitudDeIngresoPedido solicitud = RehidratarSolicitud(estado: EstadoSolicitud.Aceptada);

            //Assert
            Assert.AreEqual(EstadoSolicitud.Aceptada, solicitud.Estado);
        }

        [TestMethod]
        public void SolicitudDeIngresoPedido_ShouldPreserveMotivosDeRechazo_When_IsRehidratada()
        {
            // Arrange
            string[] motivos = ["El SKU no existe en el catálogo"];

            // Act
            SolicitudDeIngresoPedido solicitud = RehidratarSolicitud(
                estado: EstadoSolicitud.Rechazada,
                motivos: motivos);

            // Assert
            Assert.AreEqual("El SKU no existe en el catálogo", solicitud.MotivosDeRechazo.Single());
        }
    }
}
