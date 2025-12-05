using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/hello-world")]
public class HelloWorldController : Controller
{
    [HttpGet]
    public string Get()
    {
        return "Hello World";
    }
}
