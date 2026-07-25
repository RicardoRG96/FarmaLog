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
            Assert.ThrowsExactly<SolicitudIncoherenteRespesctoACenabastException>(
                () => SolicitudDeIngresoPedido.Create(true, null));
        }
    }
}
