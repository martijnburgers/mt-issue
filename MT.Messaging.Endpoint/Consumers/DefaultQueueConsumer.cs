using MassTransit;

namespace MT.Messaging.Endpoint.Consumers;

public class DefaultQueueConsumer : IConsumer<DefaultMessage>
{
    public Task Consume(ConsumeContext<DefaultMessage> context)
    {
        Console.WriteLine(typeof(DefaultMessage).FullName + " consumed");

        return Task.CompletedTask;
    }
}