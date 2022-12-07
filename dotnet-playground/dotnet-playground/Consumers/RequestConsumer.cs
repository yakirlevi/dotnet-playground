using System.Diagnostics;
using MassTransit;

namespace dotnet_playground.Consumers;

public class Request
{
    public string Name { get; set; }
    public int Counter { get; set; }
}

public class RequestConsumer : IConsumer<Request>
{
    public Task Consume(ConsumeContext<Request> context)
    {
        var message = context.Message;
        var activity = Activity.Current;
        Console.WriteLine($"{activity?.SpanId}  {activity?.TraceId}  request consumer: {message.Counter}");
        foreach (var o in activity.Baggage)
        {
            Console.WriteLine(o.Value?.ToString());
        }
        return Task.CompletedTask;
    }
}