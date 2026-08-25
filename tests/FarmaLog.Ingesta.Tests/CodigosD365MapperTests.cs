using FarmaLog.Ingesta.Publishing;

namespace FarmaLog.Ingesta.Tests
{
    [TestClass]
    public class CodigosD365MapperTests
    {
        [TestMethod]
        public void CodigosD365Mapper_Should_PadAccountRutToTenCharacters()
        {
            Assert.AreEqual("23-0775634129", CodigosD365Mapper.MapCuentaCliente("23", "775634129"));
        }

        [TestMethod]
        public void CodigosD365Mapper_Should_PadShortAccountRutWithTwoZeros()
        {
            // RUT de 8 (cuerpo de 7) es normal en clientes finales chilenos.
            Assert.AreEqual("23-009876543K", CodigosD365Mapper.MapCuentaCliente("23", "9876543K"));
        }

        [TestMethod]
        public void CodigosD365Mapper_Should_NotPadDispatchAddress()
        {
            Assert.AreEqual("23-775634129D1",
                CodigosD365Mapper.MapDireccionDespacho("23", "775634129D1"));
        }
    }
}
