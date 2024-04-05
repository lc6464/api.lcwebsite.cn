namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class GetIPController(ILogger<GetIPController> logger, IHttpConnectionInfo info, IHttp304 http304) : ControllerBase {
	[HttpGet]
	[ResponseCache(CacheProfileName = "Private1m")] // 客户端缓存1分钟
	public IP? Get() { // 获取 IP 地址
		IP ip = new(info);
		logger.LogDebug("GetIP: Client {}:{} on {}", ip.Family == "IPv6" ? $"[{ip.Address}]" : ip.Address, info.RemotePort, info.Protocol);

		return http304.TrySet(true, $"{ip.Address}|{info.RemotePort}|{info.Protocol}") ? null : new(info);
	}
}