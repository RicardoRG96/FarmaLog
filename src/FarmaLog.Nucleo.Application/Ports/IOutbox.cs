namespace FarmaLog.Nucleo.Application.Ports
{
    public interface IOutbox
    {
        void Add(OutboxMessage message);
    }
}
