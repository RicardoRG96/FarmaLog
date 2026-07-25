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

        [TestMethod]
        public void DireccionDespacho_ShouldThrow_When_LetterD_IsNotPresent()
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create("23-7789036711"));
        }

        [TestMethod]
        [DataRow("23-77890D36711")]
        [DataRow("23-7789D036711")]
        [DataRow("23-778D9036711")]
        [DataRow("23-7D789036711")]
        [DataRow("23-7789036711D")]
        [DataRow("23-D7789036711")]
        public void DireccionDespacho_ShouldThrow_When_LetterD_IsNotInTheRightPosition(
            string direccion)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccion));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldConstruct_AndNormalize_When_DireccionDespachoHasALowercaseLetterD()
        {
            //Arrange
            string expected = "23-778903671D1";

            //Act
            DireccionDespacho direccionDespacho = DireccionDespacho.Create("23-778903671d1");

            //Assert
            Assert.AreEqual(expected, direccionDespacho.Direccion);
        }

        [TestMethod]
        [DataRow("23-7789HJ671D1")]
        [DataRow("23-7789HJ67KD1")]
        [DataRow("23-.7/95667KD1")]
        public void DireccionDespacho_ShouldThrow_When_RutDoesNotContainsOnlyDigitsExceptTheLastOneChar(
            string direccion)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccion));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldConstruct_WithLastCharOfTheRutNormalized_When_RutHasALetterAsDV()
        {
            //Arrange
            string expected = "23-77890367KD1";

            //Act
            DireccionDespacho direccionDespacho = DireccionDespacho.Create("23-77890367kD1");

            //Assert
            Assert.AreEqual(expected, direccionDespacho.Direccion);
        }

        [TestMethod]
        [DataRow("23-77890367QD1")]
        [DataRow("23-77890367$D1")]
        [DataRow("23-77890367.D1")]
        public void DireccionDespacho_ShouldThrow_When_ARutHasNotANumberOrALetterKAsDigitoVerificador(
            string direccion)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccion));
        }

        [TestMethod]
        [DataRow("23-77890367KDlm")]
        [DataRow("23-77890367KD.$")]
        [DataRow("23-77890367KD/=?#,")]
        public void DireccionDespacho_ShouldThrow_When_TheCharactersAfterThe_D_AreNotNumbers(
            string direccion)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccion));
        }

        [TestMethod]
        [DataRow("23-77890367KD01")]
        [DataRow("23-77890367KD010")]
        [DataRow("23-77890367KD0018")]
        [DataRow("23-77890367KD00030")]
        public void DireccionDespacho_ShouldThrow_When_TheCharactersAfterThe_D_HasALeadingZero(
            string direccion)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccion));
        }

        [TestMethod]
        [DataRow("23-77890367KD0")]
        [DataRow("23-77890367KD-1")]
        [DataRow("23-77890367KD-2")]
        [DataRow("23-77890367KD-120")]
        public void DireccionDespacho_ShouldThrow_When_TheCharacterAfterThe_D_IsLessThanOne(
            string direccion)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccion));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldThrow_When_AreNoCharactersAfterThe_D()
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create("23-77890367KD"));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldConstruct_When_ADireccionDespachoHasACodigoLaboratorioWithALengthOf_3_OrMore()
        {
            //Arrange
            string expected = "237-78903671D1";

            //Act
            DireccionDespacho direccionDespacho = DireccionDespacho.Create("237-78903671D1");

            //Assert
            Assert.AreEqual(expected, direccionDespacho.Direccion);
        }

        [TestMethod]
        public void DireccionDespacho_ShouldThrow_When_AnEmptyStringIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(""));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldThrow_When_NullIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(null));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldThrow_When_AWhiteSpaceStringIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create("   "));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldConstruct_When_AValidStringWithWhiteSpacesIsPassed()
        {
            //Arrange
            string expected = "237-78903671D1";

            //Act
            DireccionDespacho direccionDespacho = DireccionDespacho.Create("   237-78903671D1   ");

            //Assert
            Assert.AreEqual(expected, direccionDespacho.Direccion);
        }

        [TestMethod]
        public void DireccionDespacho_ShouldConstructAndNormalize_WhenAValidStringWithWhiteSpacesAndLowerCaseLetterDIsPassed()
        {
            //Arrange
            string expected = "237-78903671D1";

            //Act
            DireccionDespacho direccionDespacho = DireccionDespacho.Create("   237-78903671d1   ");

            //Assert
            Assert.AreEqual(expected, direccionDespacho.Direccion);
        }

        [TestMethod]
        [DataRow("23- 77890367KD1")]
        [DataRow("23 -77890367KD1")]
        [DataRow("23-77890 367KD1")]
        [DataRow("23-77890367 KD1")]
        [DataRow("23-77890367K D1")]
        [DataRow("23- 77890367KD 1")]
        public void DireccionDespacho_ShouldThrow_When_AStringWithIntermediateSpacesIsPassed(
            string direccion)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccion));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldHave_ACorrectPopulatedCodigoLaboratorio_WhenIsBuilt()
        {
            //Arrange
            string expected = "23";

            //Act
            DireccionDespacho direccionDespacho = DireccionDespacho.Create("   23-78903671kD1");

            //Assert
            Assert.AreEqual(expected, direccionDespacho.Codigo.Codigo);
        }
    }
}
