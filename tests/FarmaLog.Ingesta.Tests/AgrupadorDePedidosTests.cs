namespace FarmaLog.Ingesta.Tests
{
    [TestClass]
    public class AgrupadorDePedidosTests
    {
        [TestMethod]
        public void AgrupadorDePedidos_Should_GroupRowsSharingDeliveryNumber()
        {
            //Arrange
            FilaCruda[] filas =
            [
                Filas.Una(numeroDeFila: 2, delivery: "DEL-1", sku: "SKU-A"),
                Filas.Una(numeroDeFila: 3, delivery: "DEL-2", sku: "SKU-B"),
                Filas.Una(numeroDeFila: 4, delivery: "DEL-1", sku: "SKU-C"),
            ];

            //Act
            IReadOnlyList<PedidoAgrupado> pedidos = AgrupadorDePedidos.Agrupar(filas);

            //Assert
            Assert.HasCount(2, pedidos);
            Assert.HasCount(2, pedidos.Single(p => p.NumeroDelivery == "DEL-1").Filas);
        }

        [TestMethod]
        public void AgrupadorDePedidos_Should_PreserveFirstAppearanceOrder()
        {
            //Arrange
            FilaCruda[] filas =
            [
                Filas.Una(numeroDeFila: 2, delivery: "DEL-9"),
                Filas.Una(numeroDeFila: 3, delivery: "DEL-1"),
                Filas.Una(numeroDeFila: 4, delivery: "DEL-9"),
            ];

            //Act
            IReadOnlyList<PedidoAgrupado> pedidos = AgrupadorDePedidos.Agrupar(filas);

            //Assert
            CollectionAssert.AreEqual(
                new[] { "DEL-9", "DEL-1" },
                pedidos.Select(p => p.NumeroDelivery).ToArray(),
                "El orden de los errores en pantalla debe seguir al archivo, no al alfabeto.");
        }
    }
}
