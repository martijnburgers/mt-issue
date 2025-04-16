using MassTransit.AzureServiceBusTransport;
using MassTransit.Transports;
using MassTransit;

namespace MT.Messaging.Endpoint
{
    public sealed class AzureServiceBusCompliantNameFormatter
        : IEntityNameFormatter
    {
        readonly IMessageNameFormatter _formatter = new ServiceBusMessageNameFormatter();

        public string FormatEntityName<T>()
        {
            var entityName = _formatter.GetMessageName(typeof(T)).ToString();
            return entityName;
        }
    }
}
