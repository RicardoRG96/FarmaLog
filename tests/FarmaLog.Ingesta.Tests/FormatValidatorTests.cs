namespace FarmaLog.Ingesta.Tests
{
    [TestClass]
    public class FormatValidatorTests
    {
        private static PedidoGroup Pedido(params PlanillaRow[] rows) =>
            new(rows[0].NumeroDelivery, rows);

        [TestMethod]
        public void ValidadorDeFormaTests_Should_ReportEmptyMandatoryCell()
        {
            //Arrange
            PedidoGroup pedido = Pedido(PlanillaRows.One(delivery: "DEL-1", cuenta: ""));

            //Act
            IReadOnlyList<ValidationError> errors = FormatValidator.Validate(pedido);

            //Assert
            Assert.HasCount(1, errors);
            Assert.AreEqual("DEL-1", errors[0].location);
            StringAssert.Contains(errors[0].Problem, "Cuenta de Cliente");
        }

        [TestMethod]
        public void ValidadorDeForma_Should_RejectDateNotInChileanFormat()
        {
            //Arrange
            PedidoGroup pedido = Pedido(PlanillaRows.One(delivery: "DEL-1", fecha: "09-12-2025"));

            //Act
            IReadOnlyList<ValidationError> errors = FormatValidator.Validate(pedido);

            //Assert
            Assert.HasCount(1, errors);
            Assert.AreEqual("09-12-2025", errors[0].OffendingValue);
            StringAssert.Contains(errors[0].HowToFix, "dd/MM/yyyy");
        }

        [TestMethod]
        public void ValidadorDeForma_Should_AcceptEmptyDateBecauseColumnIsOptional()
        {
            //Arrange + Act
            PedidoGroup pedido = Pedido(PlanillaRows.One(delivery: "DEL-1", fecha: ""));

            //Assert
            Assert.IsEmpty(FormatValidator.Validate(pedido));
        }

        [TestMethod]
        public void ValidadorDeForma_Should_RequireCenabastDocumentWhenOrderIsCenabast()
        {
            //Arrange
            PedidoGroup pedido = Pedido(PlanillaRows.One(delivery: "DEL-1", esCenabast: "SI", cenabast: ""));

            //Act
            IReadOnlyList<ValidationError> errors = FormatValidator.Validate(pedido);

            //Assert
            Assert.HasCount(1, errors);
            StringAssert.Contains(errors[0].Problem, "Cenabast");
        }
    }
}
