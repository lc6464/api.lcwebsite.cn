using System.Net.Sockets;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class GetIPController(ILogger<GetIPController> logger, IHttpConnectionInfo info, IHttp304 http304) : ControllerBase {
	[HttpGet]
	[ResponseCache(CacheProfileName = "Private1m")] // 客户端缓存1分钟
	public IP? Get() { // 获取 IP 地址
		var address = info.RemoteAddress;
		logger.LogDebug("GetIP: Client {}:{} on {}", address?.AddressFamily == AddressFamily.InterNetworkV6 ? $"[{address}]" : address, info.RemotePort, info.Protocol);

		return http304.TrySet(true, info.Protocol) ? null : new(info);
	}
}