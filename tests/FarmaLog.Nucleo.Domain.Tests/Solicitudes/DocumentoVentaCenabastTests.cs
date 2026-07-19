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

        [TestMethod]
        public void DocumentoVentaCenabast_ShouldThrow_When_AWhiteSpaceStringIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<DocumentoVentaCenabastInvalidoException>(
                () => DocumentoVentaCenabast.Create("   "));
        }

        [TestMethod]
        public void DocumentoVentaCenabast_Should_ConstructADocumentoVentaCenabastWithoutWhiteSpaces_When_AStringWithWhiteSpacesIsPassed()
        {
            //Arrange
            string expected = "123";

            //Act
            DocumentoVentaCenabast dvCenabast = DocumentoVentaCenabast.Create("  123  ");

            //Assert
            Assert.AreEqual(expected, dvCenabast.Valor);
        }

        [TestMethod]
        public void DocumentoVentaCenabast_ShouldConstructTheSameInput_When_AStringWithIntermediateWhiteSpacesIsPassed()
        {
            //Arrange
            string expected = "123 456";

            //Act
            DocumentoVentaCenabast dvCenabast = DocumentoVentaCenabast.Create("123 456");

            //Assert
            Assert.AreEqual(expected, dvCenabast.Valor);
        }

        [TestMethod]
        public void DocumentoVentaCenabast_Should_ConstructADocumentoVentaCenabastWithLeadingZero_When_AStringWithLeadingZeroIsPassed()
        {
            //Arrange
            string expected = "0123";

            //Act
            DocumentoVentaCenabast dvCenabast = DocumentoVentaCenabast.Create("0123");

            //Assert
            Assert.AreEqual(expected, dvCenabast.Valor);
        }
    }
}
