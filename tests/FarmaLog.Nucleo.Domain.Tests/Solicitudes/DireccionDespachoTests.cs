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

        [TestMethod]
        [DataRow("2377-8903671D1")]
        [DataRow("2377890-3671D1")]
        [DataRow("23778-903671D112")]
        [DataRow("23778903671D112-")]
        [DataRow("-23778903671D112")]
        public void DireccionDespacho_ShouldThrow_When_DashIsNotInTheRightPosition(
            string direccionDespacho)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccionDespacho));
        }
    }
}
