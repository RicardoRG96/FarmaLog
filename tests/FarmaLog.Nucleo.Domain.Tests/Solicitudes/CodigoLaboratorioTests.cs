using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class CodigoLaboratorioTests
    {
        [TestMethod]
        public void CodigoLaboratorio_ShouldThrow_When_AStringWithLettersIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<CodigoLaboratorioInvalidoException>(() => CodigoLaboratorio.Create("BI"));
        }

        [TestMethod]
        public void CodigoLaboratorio_ShouldThrow_When_AStringWithValue_XY_IsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<CodigoLaboratorioInvalidoException>(() => CodigoLaboratorio.Create("XY"));
        }
    }
}
