using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class LineaSolicitudTests
    {
        [TestMethod]
        public void LineaSolicitud_ShouldThrow_When_LineaSolicitudHasAQuantityOfZero()
        {
            //Act + Assert
            Assert.ThrowsExactly<LineaSolicitudInvalidaException>(
                () => LineaSolicitud.Create("SKU-1", 0));
        }
    }
}
