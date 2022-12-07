using dotnet_playground.Consumers;
using MassTransit;
using Microsoft.Extensions.Hosting;
using static System.Threading.Tasks.Task;

namespace dotnet_playground;

public class Worker : IHostedService
{
    private IBus _bus;
    public Worker(IBus bus)
    {
        _bus = bus;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Run(() => Do(cancellationToken));
        return CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return CompletedTask;
    }

    public async Task Do(CancellationToken cancellationToken)
    {
        Console.WriteLine("worker started");
        await PublishLoop(cancellationToken);
        // await RestLoop();
        Console.WriteLine("worker ended");
    }

    private async Task RestLoop()
    {
        while (true)
        {
            var uri = new Uri($"http://localhost:5234/api");
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            var httpClient = new HttpClient();
            var res = await httpClient.SendAsync(requestMessage);
            await Delay(1000 * 5);
        }
    }
    
    private async Task PublishLoop(CancellationToken cancellationToken)
    {
        var req = new ChainRequest
        {
            Counter = 0
        };
        while (true)
        {
            req.Counter++;
            await _bus.Publish(req, cancellationToken);
            await Delay(1000 * 5, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                break;
        }
    }
}