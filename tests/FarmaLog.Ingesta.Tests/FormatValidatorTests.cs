using FarmaLog.Ingesta.Planilla;
using FarmaLog.Ingesta.Validation;

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
            Assert.AreEqual("DEL-1", errors[0].Location);
            Assert.Contains("Cuenta de Cliente", errors[0].Problem);
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
            Assert.Contains("dd/MM/yyyy", errors[0].HowToFix);
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
            Assert.Contains("Cenabast", errors[0].Problem);
        }

        [TestMethod]
        public void FormatValidator_Should_ReportOneErrorPerRule_When_ManyRowsShareTheSameViolation()
        {
            // Arrange — dos líneas del mismo pedido, ambas sin SKU
            PedidoGroup pedido = Pedido(
                PlanillaRows.One(delivery: "DEL-1", sku: ""),
                PlanillaRows.One(delivery: "DEL-1", sku: ""));

            // Act
            IReadOnlyList<ValidationError> errors = FormatValidator.Validate(pedido);

            // Assert
            Assert.HasCount(1, errors);
            Assert.Contains("Código de Artículo", errors[0].Problem);
        }

        [TestMethod]
        public void FormatValidator_Should_ReportHeaderRuleOncePerPedido_When_PedidoHasManyRows()
        {
            // Arrange — dos líneas del mismo pedido, misma fecha inválida en ambas
            PedidoGroup pedido = Pedido(
                PlanillaRows.One(delivery: "DEL-1", fecha: "22-09-2026"),
                PlanillaRows.One(delivery: "DEL-1", fecha: "22-09-2026"));

            // Act
            IReadOnlyList<ValidationError> errors = FormatValidator.Validate(pedido);

            // Assert
            Assert.HasCount(1, errors);
            Assert.Contains("Formato de fecha", errors[0].Problem);
        }
    }
}
