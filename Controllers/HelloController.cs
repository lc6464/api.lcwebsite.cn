using System.Net.Sockets;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase {
	private readonly ILogger<HelloController> _logger;
	private readonly IHttpConnectionInfo _info;

	public HelloController(ILogger<HelloController> logger, IHttpConnectionInfo info) {
		_logger = logger;
		_info = info;
	}

	[HttpGet]
	[ResponseCache(CacheProfileName = "NoStore")]
	public Hello Get() { // 打个招呼
		var address = _info.RemoteAddress;
		_logger.LogDebug("Hello! Client {}:{} on {}", address?.AddressFamily == AddressFamily.InterNetworkV6 ? $"[{address}]" : address, _info.RemotePort, _info.Protocol);
		return new(_info);
	}
}