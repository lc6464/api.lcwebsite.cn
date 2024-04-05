namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class GetUAController(ILogger<GetIPController> logger, IHttp304 http304) : ControllerBase {
	[HttpGet]
[ResponseCache(CacheProfileName = "Private1d")] // 客户端缓存1天
public UserAgent? Get() { // 获取 User-Agent
	UserAgent ua = new(Request.Headers);
	logger.LogDebug("GetUA: Client using {} on {}, is mobile: {} ({})", ua.Sec_CH_UA, ua.Sec_CH_UA_Platform, ua.Sec_CH_UA_Mobile, ua.UA);

	return http304.TrySet(true, $"{ua.Sec_CH_UA}|{ua.Sec_CH_UA_Platform}|{ua.Sec_CH_UA_Mobile}|{ua.UA}") ? null : ua;
}
}