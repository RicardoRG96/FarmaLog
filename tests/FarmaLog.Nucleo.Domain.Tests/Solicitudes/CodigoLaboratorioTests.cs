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
        [DataRow("XY")]
        [DataRow("A1")]
        public void CodigoLaboratorio_ShouldThrow_When_AStringThatDoesNotContainOnlyIntegers_IsPassed(string codigo)
        {
            //Act + Assert
            Assert.ThrowsExactly<CodigoLaboratorioInvalidoException>(() => CodigoLaboratorio.Create(codigo));
        }
    }
}
