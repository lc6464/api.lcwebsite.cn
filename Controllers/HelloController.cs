namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class HelloController(ILogger<HelloController> logger, IHttpConnectionInfo info) : ControllerBase {
	[HttpGet]
	[ResponseCache(CacheProfileName = "NoStore")]
	public Hello Get() { // 打个招呼
		IP ip = new(info);
		logger.LogDebug("Hello! Client {}:{} on {} using {}", ip.Family == "IPv6" ? $"[{ip.Address}]" : ip.Address, info.RemotePort, info.Protocol, Request.Headers.UserAgent);

		return new(ip, new(Request.Headers));
	}
}