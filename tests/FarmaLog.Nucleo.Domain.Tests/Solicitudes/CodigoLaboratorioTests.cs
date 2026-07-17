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
        public void CodigoLaboratorio_ShouldThrow_When_AStringThatDoesNotContainOnlyIntegersIsPassed(string codigo)
        {
            //Act + Assert
            Assert.ThrowsExactly<CodigoLaboratorioInvalidoException>(() => CodigoLaboratorio.Create(codigo));
        }

        [TestMethod]
        public void CodigoLaboratorio_ShouldThrow_When_AStringWithALengthLessThanTwoIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<CodigoLaboratorioInvalidoException>(() => CodigoLaboratorio.Create("1"));
        }

        [TestMethod]
        public void CodigoLaboratorio_ShouldConstruct_When_AStringWithALengthOfTwoIsPassed()
        {
            //Arrange
            string expectedCodigo = "23";

            //Act
            CodigoLaboratorio codigoLab = CodigoLaboratorio.Create("23");

            //Assert
            Assert.AreEqual(expectedCodigo, codigoLab.Codigo);
        }
    }
}
