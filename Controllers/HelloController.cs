using System.Net.Sockets;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class HelloController(ILogger<HelloController> logger, IHttpConnectionInfo info) : ControllerBase {
	[HttpGet]
	[ResponseCache(CacheProfileName = "NoStore")]
	public Hello Get() { // 打个招呼
		var address = info.RemoteAddress;
		logger.LogDebug("Hello! Client {}:{} on {}", address?.AddressFamily == AddressFamily.InterNetworkV6 ? $"[{address}]" : address, info.RemotePort, info.Protocol);
		return new(info);
	}
}