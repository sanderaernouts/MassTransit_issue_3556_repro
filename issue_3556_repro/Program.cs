using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace issue_3556_repro;

public static class Program
{

    public static async Task Main(string[] args)
    {
        var subscription = DiagnosticListener.AllListeners.Subscribe(new DiagnosticObserver());
        await Host.CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureServices(services =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumersFromNamespaceContaining<TestConsumer>();
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                });

                services.AddHostedService<TestService>();
            })
            .Build()
            .RunAsync();
    }

    public class DiagnosticObserver : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object?>>
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object?> value)
        {
            Console.WriteLine($"{value.Key}={value.Value}");
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == "MassTransit")
            {
                value.Subscribe(this);
            }
        }
    }
}