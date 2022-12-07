using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_playground.Controllers;

[Route("api")]
public class FirstController : Controller
{
    [HttpPost("one")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> One()
    {
        var activity = Activity.Current;
        activity?.AddBaggage("one1", "oneval");
        activity?.AddBaggage("one2", "oneval");
        Console.WriteLine($"{activity?.Id}  {activity?.TraceId} FirstController: One");
        
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"http://localhost:5234/api/two"),
            Method = HttpMethod.Post
        };
        var httpClient = new HttpClient();
        var res = await httpClient.SendAsync(requestMessage);
        
        return Ok();
    }
    
    [HttpPost("two")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Two()
    {
        var context = HttpContext;
        var headers = context.Request.Headers;
        var activity = Activity.Current;
        Console.WriteLine($"{activity?.Id}  {activity?.TraceId} FirstController: Two");
        foreach (var tagObject in activity.Baggage)
        {
            Console.WriteLine(tagObject.Value?.ToString());
        }
        await Task.Delay(30);
        return Ok();
    }
}