using FarmaLog.Ingesta.Planilla;

namespace FarmaLog.Ingesta.Tests
{
    [TestClass]
    public class PedidoGrouperTests
    {
        [TestMethod]
        public void AgrupadorDePedidos_Should_GroupRowsSharingDeliveryNumber()
        {
            //Arrange
            PlanillaRow[] filas =
            [
                PlanillaRows.One(numeroDeFila: 2, delivery: "DEL-1", sku: "SKU-A"),
                PlanillaRows.One(numeroDeFila: 3, delivery: "DEL-2", sku: "SKU-B"),
                PlanillaRows.One(numeroDeFila: 4, delivery: "DEL-1", sku: "SKU-C"),
            ];

            //Act
            IReadOnlyList<PedidoGroup> pedidos = PedidoGrouper.Group(filas);

            //Assert
            Assert.HasCount(2, pedidos);
            Assert.HasCount(2, pedidos.Single(p => p.NumeroDelivery == "DEL-1").Rows);
        }

        [TestMethod]
        public void AgrupadorDePedidos_Should_PreserveFirstAppearanceOrder()
        {
            //Arrange
            PlanillaRow[] filas =
            [
                PlanillaRows.One(numeroDeFila: 2, delivery: "DEL-9"),
                PlanillaRows.One(numeroDeFila: 3, delivery: "DEL-1"),
                PlanillaRows.One(numeroDeFila: 4, delivery: "DEL-9"),
            ];

            //Act
            IReadOnlyList<PedidoGroup> pedidos = PedidoGrouper.Group(filas);

            //Assert
            CollectionAssert.AreEqual(
                new[] { "DEL-9", "DEL-1" },
                pedidos.Select(p => p.NumeroDelivery).ToArray(),
                "El orden de los errores en pantalla debe seguir al archivo, no al alfabeto.");
        }
    }
}
