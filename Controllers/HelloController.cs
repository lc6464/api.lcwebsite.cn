using System.Net.Sockets;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase {
	private readonly ILogger<HelloController> _logger;

	public HelloController(ILogger<HelloController> logger) {
		_logger = logger;
	}

	[HttpGet]
	public Hello Get() { // 打个招呼
		var connection = HttpContext.Connection;
		var address = connection.RemoteIpAddress;
		_logger.LogDebug("Hello! Client {}:{}", address?.AddressFamily == AddressFamily.InterNetworkV6 ? $"[{address}]" : address, connection.RemotePort);
		return new();
	}
}