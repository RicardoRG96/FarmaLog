namespace FarmaLog.Ingesta.Tests
{
    [TestClass]
    public class PlanillaReaderTests
    {
        private const string FileName = "template-real.xlsx";
        private static Stream OpenFixture(string nombre) =>
            File.OpenRead(Path.Combine("Fixtures", nombre));

        [TestMethod]
        public void PlanillaReader_Should_IgnoreFormattedButEmptyRows()
        {
            //Arrange
            PlanillaReader reader = new();
            Stream planilla = OpenFixture(FileName);

            //Act
            IReadOnlyList<FilaCruda> rows = reader.Read(planilla);

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
    }
}
