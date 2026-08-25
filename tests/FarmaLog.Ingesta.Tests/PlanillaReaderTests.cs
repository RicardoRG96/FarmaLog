using FarmaLog.Ingesta.Planilla;

namespace FarmaLog.Ingesta.Tests
{
    [TestClass]
    public class PlanillaReaderTests
    {
        private const string FileName = "template-real.xlsx";
        private static Stream OpenFixture(string name) =>
            File.OpenRead(Path.Combine("Fixtures", name));

        [TestMethod]
        public void PlanillaReader_Should_IgnoreFormattedButEmptyRows()
        {
            //Arrange
            PlanillaReader reader = new();
            Stream planilla = OpenFixture(FileName);

            //Act
            IReadOnlyList<PlanillaRow> rows = reader.Read(planilla);

            //Assert
            Assert.HasCount(1, rows,
                "La hoja reporta 17 filas; solo la 2 trae datos. Las demás están formateadas y vacías.");
        }

        [TestMethod]
        public void PlanillaReader_Should_MatchExpectedHeaderLayout()
        {
            //Arrange
            string[] expected =
            [
                "numero pedido del laboratorio", "cuenta de cliente",
                "documento de venta cenabast", "codigo de articulo", "cantidad",
                "lote (opcional)", "codigo de despacho", "numero de orden de compra",
                "estado del inventario", "pedido cenabast (si o no)",
                "fecha de entrega dd/mm/aaaa", "urgencia (si o no)", "observacion"
            ];

            using Stream planilla = OpenFixture(FileName);

            //Act
            IReadOnlyList<string> real = PlanillaReader.ReadNormalizedHeaders(planilla);

            //Assert
            CollectionAssert.AreEqual(expected, real.ToArray(),
                "El template cambió de forma. Las columnas se leen por posición fija.");
        }

        [TestMethod]
        public void PlanillaReader_Should_KeepRowWithDataButNoDeliveryNumber()
        {
            using var planilla = OpenFixture("fixture-fila-sin-delivery.xlsx");

            var filas = new PlanillaReader().Read(planilla);

            Assert.HasCount(2, filas,
                "Una fila con datos y sin Delivery es un error del usuario, no una fila fantasma.");
            Assert.AreEqual("", filas[1].NumeroDelivery);
        }
    }
}
