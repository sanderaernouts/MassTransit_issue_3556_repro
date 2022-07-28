using MassTransit;
using Microsoft.Extensions.Hosting;

namespace issue_3556_repro;

public class TestService : IHostedService
{
    private readonly ISendEndpointProvider provider;

    public TestService(ISendEndpointProvider provider)
    {
        this.provider = provider;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var endpoint = await provider.GetSendEndpoint(new Uri("loopback://localhost/Test"));

        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                await endpoint.Send(new TestMessage
                {
                    Id = Guid.NewGuid()
                });
            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}