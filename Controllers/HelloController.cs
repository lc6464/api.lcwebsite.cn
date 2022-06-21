using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
	[ApiController]
	[Route("[controller]")]
	public class HelloController : ControllerBase {
		private readonly ILogger<HelloController> _logger;

		public HelloController(ILogger<HelloController> logger) {
			_logger = logger;
		}

		[HttpGet]
		public Hello Get() {
			return new();
		}
	}
}