using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class DocumentoVentaCenabastTests
    {
        [TestMethod]
        public void DocumentoVentaCenabast_ShouldThrow_When_AnEmptyStringIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<DocumentoVentaCenabastInvalidoException>(
                () => DocumentoVentaCenabast.Create(""));
        }

        [TestMethod]
        public void DocumentoVentaCenabast_ShouldThrow_When_NullIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<DocumentoVentaCenabastInvalidoException>(
                () => DocumentoVentaCenabast.Create(null));
        }
    }
}
