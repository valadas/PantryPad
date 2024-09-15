using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class SimpleController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(){
        return Ok(new { message = "Hello from API", date = DateTime.UtcNow });
    }
}