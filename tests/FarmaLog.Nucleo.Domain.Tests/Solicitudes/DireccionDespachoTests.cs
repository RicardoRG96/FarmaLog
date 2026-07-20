using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class DireccionDespachoTests
    {
        [TestMethod]
        [DataRow("BI")]
        [DataRow("A1")]
        [DataRow("1")]
        public void DireccionDespacho_ShouldThrow_When_CodigoLaboratorioIsInvalid(
            string codigoLaboratorio)
        {
            //Arrange
            string formatedDireccionDespacho = $"{codigoLaboratorio}-778903671D1";

            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(formatedDireccionDespacho));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldThrow_When_DashIsNotPresent()
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create("23778903671D1"));
        }
    }
}
