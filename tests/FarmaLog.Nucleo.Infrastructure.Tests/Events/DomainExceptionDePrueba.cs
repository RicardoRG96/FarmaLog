using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Events
{
    public sealed class DomainExceptionDePrueba : DomainException
    {
        public DomainExceptionDePrueba(string message) 
            : base(message) {}
    }
}
