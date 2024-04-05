namespace API.Controllers;
[ApiController]
[Route(@"[controller]/{id:length(3,20):regex(^(av(\d+)|BV[[A-Za-z0-9]]+)$)}")]
public class BiliVideoInfoController(ILogger<BiliVideoInfoController> logger, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IHttp304 http304) : ControllerBase {
	[HttpGet]
[ResponseCache(CacheProfileName = "Public1d")] // 与绝对过期时间匹配
public async Task<BiliVideoInfo?> Get(string id) {
	var cacheKey = "BiliVideoInfoAPI-" + id;
	if (memoryCache.TryGetValue(cacheKey, out BiliVideoInfo info)) {
		logger.LogDebug("已命中内存缓存：{}: {}", cacheKey, JsonSerializer.Serialize(info));

		return http304.TrySet(info.Code.ToString()) ? null : info;
	}

	var queryString = id.StartsWith("av") ? $"?aid={id.AsSpan(2)}" : $"?bvid={id}";
	using var hc = httpClientFactory.CreateClient("Timeout5s");
	hc.BaseAddress = new("https://api.bilibili.com/x/web-interface/view");
	try {
		var start = DateTime.UtcNow;
		info = await hc.GetFromJsonAsync<BiliVideoInfo>(queryString).ConfigureAwait(false);
		Response.Headers.Append("Server-Timing", $"g;desc=\"Get API\";dur={(DateTime.UtcNow - start).TotalMilliseconds}"); // Server Timing API

		using var entry = memoryCache.CreateEntry(cacheKey); // 写入内存缓存
		entry.Value = info;
		entry.SlidingExpiration = TimeSpan.FromMinutes(15); // 滑动过期15分钟
		entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1); // 绝对过期1天
		logger.LogDebug("已写入内存缓存：{}: {}", cacheKey, JsonSerializer.Serialize(info));

		return http304.TrySet(info.Code.ToString()) ? null : info;
	} catch (Exception e) {
		logger.LogCritical("在 Get {} 时连接至哔哩哔哩服务器过程中发生异常：{}", id, e);
		return new() { Code = 1, Message = "无法连接哔哩哔哩服务器！" };
	}
}
}