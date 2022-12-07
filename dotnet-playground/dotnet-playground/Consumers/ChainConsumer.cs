using System.Diagnostics;
using MassTransit;

namespace dotnet_playground.Consumers;

public class ChainRequest
{
    public int Counter { get; set; }
}

public class ChainConsumer : IConsumer<ChainRequest>
{
    public async Task Consume(ConsumeContext<ChainRequest> context)
    {
        var message = context.Message;
        var activity = Activity.Current;
        activity?.AddBaggage("user", "smith");
        Console.WriteLine($"{activity?.Id}  {activity?.TraceId} chain consumer: {message.Counter}");
        
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"http://localhost:5234/api/one"),
            Method = HttpMethod.Post
        };
        var httpClient = new HttpClient();
        // var res = await httpClient.SendAsync(requestMessage);
        await context.Publish(new Request());
    }
}