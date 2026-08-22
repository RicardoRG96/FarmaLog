namespace FarmaLog.Ingesta.Tests
{
    [TestClass]
    public class ValidadorDeFormaTests
    {
        private static PedidoAgrupado Pedido(params FilaCruda[] filas) =>
            new(filas[0].NumeroDelivery, filas);

        [TestMethod]
        public void ValidadorDeFormaTests_Should_ReportEmptyMandatoryCell()
        {
            //Arrange
            PedidoAgrupado pedido = Pedido(Filas.Una(delivery: "DEL-1", cuenta: ""));

            //Act
            IReadOnlyList<ErrorDeValidacion> errores = ValidadorDeForma.Validar(pedido);

            //Assert
            Assert.HasCount(1, errores);
            Assert.AreEqual("DEL-1", errores[0].Ubicacion);
            StringAssert.Contains(errores[0].Problema, "Cuenta de Cliente");
        }

        [TestMethod]
        public void ValidadorDeForma_Should_RejectDateNotInChileanFormat()
        {
            //Arrange
            PedidoAgrupado pedido = Pedido(Filas.Una(delivery: "DEL-1", fecha: "09-12-2025"));

            //Act
            IReadOnlyList<ErrorDeValidacion> errores = ValidadorDeForma.Validar(pedido);

            //Assert
            Assert.HasCount(1, errores);
            Assert.AreEqual("09-12-2025", errores[0].ValorOfensor);
            StringAssert.Contains(errores[0].ComoCorregir, "dd/MM/yyyy");
        }

        [TestMethod]
        public void ValidadorDeForma_Should_AcceptEmptyDateBecauseColumnIsOptional()
        {
            //Arrange + Act
            PedidoAgrupado pedido = Pedido(Filas.Una(delivery: "DEL-1", fecha: ""));

            //Assert
            Assert.IsEmpty(ValidadorDeForma.Validar(pedido));
        }

        [TestMethod]
        public void ValidadorDeForma_Should_RequireCenabastDocumentWhenOrderIsCenabast()
        {
            //Arrange
            PedidoAgrupado pedido = Pedido(Filas.Una(delivery: "DEL-1", esCenabast: "SI", cenabast: ""));

            //Act
            IReadOnlyList<ErrorDeValidacion> errores = ValidadorDeForma.Validar(pedido);

            //Assert
            Assert.HasCount(1, errores);
            StringAssert.Contains(errores[0].Problema, "Cenabast");
        }
    }
}
