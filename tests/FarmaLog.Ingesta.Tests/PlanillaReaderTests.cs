namespace FarmaLog.Ingesta.Tests
{
    [TestClass]
    public class PlanillaReaderTests
    {
        private static Stream OpenFixture(string nombre) =>
            File.OpenRead(Path.Combine("Fixtures", nombre));

        [TestMethod]
        public void PlanillaReader_Should_IgnoreFormattedButEmptyRows()
        {
            //Arrange
            PlanillaReader reader = new();
            Stream planilla = OpenFixture("template-real.xlsx");

            //Act
            IReadOnlyList<FilaCruda> rows = reader.Read(planilla);

            //Assert
            Assert.HasCount(1, rows,
                "La hoja reporta 17 filas; solo la 2 trae datos. Las demás están formateadas y vacías.");
        }
    }
}
