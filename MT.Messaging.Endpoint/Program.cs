using MassTransit;
using MassTransit.Contracts.JobService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MT.Messaging.Endpoint.Consumers;

namespace MT.Messaging.Endpoint;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddMassTransit(configurator =>
            {
                //RegisterConsumers(configurator);

                configurator.AddConsumersFromNamespaceContaining<DefaultQueueConsumer>();

                //configurator.AddConsumer<DefaultQueueConsumer>();
                //configurator.AddConsumer<SpecificQueueConsumer>();

                configurator.AddServiceBusMessageScheduler();

                configurator.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.UseServiceBusMessageScheduler();

                    var url = "sb://xxxxxxxxxx.servicebus.windows.net/";

                    Console.WriteLine($"Connecting MT to Azure Service Bus host '{url}'.");

                    cfg.Host(url);
                    cfg.MessageTopology.SetEntityNameFormatter(new AzureServiceBusCompliantNameFormatter());
                    cfg.UseNewtonsoftJsonDeserializer();
                    cfg.UseNewtonsoftJsonSerializer();

                    cfg.ReceiveEndpoint("specific",
                        (IReceiveEndpointConfigurator e) =>
                        {
                            e.ConcurrentMessageLimit = 3;
                            e.ConfigureConsumer<SpecificQueueConsumer>(context);
                        });

                    cfg.ReceiveEndpoint("default", ec =>
                    {
                        ec.ConfigureConsumers(context);
                        ec.ConfigureSagas(context);
                    });
                });
            });

            var app = builder.Build();

            Console.WriteLine("Done configuring app. About to run app.");

            await app.StartAsync();

            var bus = app.Services.GetService<IBus>();

            Console.WriteLine("Wait for bus started");

            Console.ReadKey();

            await bus.Publish<SpecificMessage>(new SpecificMessage("Yooo specific"));
            await bus.Publish<DefaultMessage>(new DefaultMessage("Yooo default"));

            Console.ReadKey();
            //app.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unhandled exception in Program.cs");
            Console.WriteLine(ex);
        }
        finally
        {
            Console.WriteLine("Shut down complete");
        }
    }
}