using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class TipoOrdenVentaTests
    {
        [TestMethod]
        public void TipoOrdenVenta_ShouldThrow_When_AnEmptyStringIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create(""));
        }

        [TestMethod]
        public void TipoOrdenVenta_ShouldThrow_When_NullIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create(null));
        }

        [TestMethod]
        public void TipoOrdenVenta_ShouldThrow_When_AWhiteSpaceStringIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create("   "));
        }

        [TestMethod]
        public void TipoOrdenVenta_ShouldThrow_When_TheLetterOfTipoOrdenIsNotPresent()
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create("231"));
        }

        [TestMethod]
        public void TipoOrdenVenta_ShouldThrow_When_TheLetterOfTipoOrdenIsNot_F_Or_G()
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create("23C5"));
        }

        [TestMethod]
        [DataRow("BI")]
        [DataRow("A1")]
        [DataRow("1")]
        public void TipoOrdenVenta_ShouldThrow_When_CodigoLaboratorioIsInvalid(
            string codigoLaboratorio)
        {
            //Arrange
            string formatedTipoOrdenVenta = $"{codigoLaboratorio}F1";

            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create(codigoLaboratorio));
        }

        [TestMethod]
        [DataRow("23F")]
        [DataRow("23G")]
        [DataRow("23C")]
        [DataRow("23D")]
        [DataRow("23P")]
        public void TipoOrdenVenta_ShouldThrow_When_ThereIsNothingAfterTheLetterOfTipoOrden(
            string tipoOrden)
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create(tipoOrden));
        }
    }
}
