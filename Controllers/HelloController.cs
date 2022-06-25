using System.Net.Sockets;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase {
	private readonly ILogger<HelloController> _logger;
	private readonly IHttpConnectionInfo _connection;

	public HelloController(ILogger<HelloController> logger, IHttpConnectionInfo connection) {
		_logger = logger;
		_connection = connection;
	}

	[HttpGet]
	[ResponseCache(CacheProfileName = "NoCache")]
	public Hello Get() { // 打个招呼
		var address = _connection.RemoteAddress;
		_logger.LogDebug("Hello! Client {}:{}", address?.AddressFamily == AddressFamily.InterNetworkV6 ? $"[{address}]" : address, _connection.RemotePort);
		return new();
	}
}