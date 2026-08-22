namespace FarmaLog.Ingesta.Tests
{
    [TestClass]
    public class FormatValidatorTests
    {
        private static PedidoGroup Pedido(params PlanillaRow[] filas) =>
            new(filas[0].NumeroDelivery, filas);

        [TestMethod]
        public void ValidadorDeFormaTests_Should_ReportEmptyMandatoryCell()
        {
            //Arrange
            PedidoGroup pedido = Pedido(PlanillaRows.Una(delivery: "DEL-1", cuenta: ""));

            //Act
            IReadOnlyList<ValidationError> errores = FormatValidator.Validar(pedido);

            //Assert
            Assert.HasCount(1, errores);
            Assert.AreEqual("DEL-1", errores[0].Ubicacion);
            StringAssert.Contains(errores[0].Problema, "Cuenta de Cliente");
        }

        [TestMethod]
        public void ValidadorDeForma_Should_RejectDateNotInChileanFormat()
        {
            //Arrange
            PedidoGroup pedido = Pedido(PlanillaRows.Una(delivery: "DEL-1", fecha: "09-12-2025"));

            //Act
            IReadOnlyList<ValidationError> errores = FormatValidator.Validar(pedido);

            //Assert
            Assert.HasCount(1, errores);
            Assert.AreEqual("09-12-2025", errores[0].ValorOfensor);
            StringAssert.Contains(errores[0].ComoCorregir, "dd/MM/yyyy");
        }

        [TestMethod]
        public void ValidadorDeForma_Should_AcceptEmptyDateBecauseColumnIsOptional()
        {
            //Arrange + Act
            PedidoGroup pedido = Pedido(PlanillaRows.Una(delivery: "DEL-1", fecha: ""));

            //Assert
            Assert.IsEmpty(FormatValidator.Validar(pedido));
        }

        [TestMethod]
        public void ValidadorDeForma_Should_RequireCenabastDocumentWhenOrderIsCenabast()
        {
            //Arrange
            PedidoGroup pedido = Pedido(PlanillaRows.Una(delivery: "DEL-1", esCenabast: "SI", cenabast: ""));

            //Act
            IReadOnlyList<ValidationError> errores = FormatValidator.Validar(pedido);

            //Assert
            Assert.HasCount(1, errores);
            StringAssert.Contains(errores[0].Problema, "Cenabast");
        }
    }
}
