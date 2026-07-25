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

        [TestMethod]
        [DataRow("23FA")]
        [DataRow("23F1X")]
        [DataRow("23F1G2")]
        [DataRow("23F1g2F1")]
        [DataRow("23FH")]
        [DataRow("23gp")]
        [DataRow("23F124G")]
        public void TipoOrdenVenta_ShouldThrow_When_TheCharactersAfterTheLetterAreNotOnlyNumbers(
            string tipoOrden)
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create(tipoOrden));
        }

        [TestMethod]
        [DataRow("23F0")]
        [DataRow("23G0")]
        [DataRow("23C0")]
        public void TipoOrdenVenta_ShouldThrow_When_TheCharactersAfterTheLetterIsZero(
            string tipoOrden)
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create(tipoOrden));
        }

        [TestMethod]
        [DataRow("23F01")]
        [DataRow("23F012")]
        [DataRow("23G020")]
        [DataRow("23G01")]
        [DataRow("23C01")]
        public void TipoOrdenVenta_ShouldThrow_When_TheCharactersAfterTheLetterHaveALeadingZero(
            string tipoOrden)
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create(tipoOrden));
        }

        [TestMethod]
        public void TipoOrdenVenta_ShouldConstructAndNormalize_When_AValidStringWithLowerCaseLetterIsPassed()
        {
            //Arrange
            string expected = "23F1";

            //Act
            TipoOrdenVenta tipoOrdenVenta = TipoOrdenVenta.Create("23f1");

            //Assert
            Assert.AreEqual(expected, tipoOrdenVenta.Tipo);
        }

        [TestMethod]
        [DataRow("23G6")]
        [DataRow("23F1")]
        [DataRow("23G8")]
        [DataRow("23G3")]
        public void TipoOrdenVenta_ShouldHave_ACorrectPopulatedCodigoLaboratorio_WhenIsBuilt(
            string tipoOrden)
        {
            //Arrange
            string expected = "23";

            //Act
            TipoOrdenVenta tipoOrdenVenta = TipoOrdenVenta.Create(tipoOrden);

            //Assert
            Assert.AreEqual(expected, tipoOrdenVenta.Codigo.Codigo);
        }

        [TestMethod]
        public void TipoOrdenVenta_ShouldThrow_When_AStringWithoutCodigoLaboratorioIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create("F1"));
        }

        [TestMethod]
        public void TipoOrdenVenta_ShouldConstruct_When_AValidStringWithWhiteSpacesIsPassed()
        {
            //Arrange
            string expected = "23F1";

            //Act
            TipoOrdenVenta tipoOrdenVenta = TipoOrdenVenta.Create("  23F1  ");

            //Assert
            Assert.AreEqual(expected, tipoOrdenVenta.Tipo);
        }

        [TestMethod]
        public void TipoOrdenVenta_ShouldConstructAndNormalize_WhenAValidStringWithWhiteSpacesAndLowerCaseLetterIsPassed()
        {
            //Arrange
            string expected = "23F1";

            //Act
            TipoOrdenVenta tipoOrdenVenta = TipoOrdenVenta.Create("  23f1  ");

            //Assert
            Assert.AreEqual(expected, tipoOrdenVenta.Tipo);
        }

        [TestMethod]
        public void TipoOrdenVenta_ShouldThrow_When_AStringWithIntermediateSpacesIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create("23 F1"));
        }
    }
}
