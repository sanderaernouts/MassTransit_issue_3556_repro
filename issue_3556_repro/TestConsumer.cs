using MassTransit;

namespace issue_3556_repro;

public class TestConsumer : IConsumer<TestMessage>
{
    public Task Consume(ConsumeContext<TestMessage> context)
    {
        Console.WriteLine($"Got {context.Message.Id}");
        return Task.CompletedTask;
    }
}