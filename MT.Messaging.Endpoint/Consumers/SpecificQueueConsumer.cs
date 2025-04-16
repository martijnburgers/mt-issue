using MassTransit;

namespace MT.Messaging.Endpoint.Consumers;

public class SpecificQueueConsumer : IConsumer<SpecificMessage>
{
    public Task Consume(ConsumeContext<SpecificMessage> context)
    {
        Console.WriteLine(typeof(SpecificMessage).FullName + " consumed");

        return Task.CompletedTask;
    }
}