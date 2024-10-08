// Copyright (c) Daniel Valadas. All rights reserved.

namespace PantryPad.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// A simple REST Api controller for initial testing.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class SimpleController : ControllerBase
    {
        /// <summary>
        /// A simple endpoint for testing.
        /// </summary>
        /// <returns>A simple message with the current date.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return this.Ok(new { message = "Hello from API 2", date = DateTime.UtcNow });
        }
    }
}
