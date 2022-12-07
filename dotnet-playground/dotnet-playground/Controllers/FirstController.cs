using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_playground.Controllers;

[Route("api")]
public class FirstController : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostIt()
    {
        var activity = Activity.Current;
        Console.WriteLine($"{activity?.Id}.{activity?.TraceId} FirstController: PostIt");
        await Task.Delay(30);
        return Ok();
    }
}