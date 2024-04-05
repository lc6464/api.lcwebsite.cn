using System.Text.RegularExpressions;

namespace API.Controllers;
[ApiController]
[Route(@"[controller]/{qq:length(5,13):regex(^\d{{5,13}}$)}")]
public partial class QQNameController(ILogger<QQNameController> logger, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IHttp304 http304) : ControllerBase {
	[HttpGet]
[ResponseCache(CacheProfileName = "Private10m")] // 与绝对过期时间匹配
public async Task<QQName?> GetAsync(string qq) {

	var cacheKey = "QQNameAPI-" + qq;
	if (memoryCache.TryGetValue(cacheKey, out string? qqName)) {
		logger.LogDebug("已命中内存缓存：{}: {}", cacheKey, qqName);

		return http304.TrySet($"{qqName == null}|{qqName}") // skipcq: CS-R1114 Visual Studio 代码清理盛情难却啊。
			? null
			: qqName == null
				? new() { Code = 2, Message = "无此 QQ 账号！", IsCache = true }
				: new() { Code = 0, Name = qqName, IsCache = true };
	}

	using var hc = httpClientFactory.CreateClient("Timeout5s");
	hc.BaseAddress = new("https://users.qzone.qq.com/fcg-bin/cgi_get_portrait.fcg");
	try {
		var start = DateTime.UtcNow;
		var result = await hc.GetStringAsync("?uins=" + qq).ConfigureAwait(false);
		Response.Headers.Append("Server-Timing", $"g;desc=\"Get API\";dur={(DateTime.UtcNow - start).TotalMilliseconds}"); // Server Timing API
		Regex head = new(@$"portraitCallBack\(\{{""{qq}"":\[""http://qlogo\d\d?\.store\.qq\.com/qzone/{qq}/{qq}/100"",((\-)?\d{{1,8}},){{5}}""");

		using var entry = memoryCache.CreateEntry(cacheKey); // 创建内存缓存
		entry.SlidingExpiration = TimeSpan.FromMinutes(2); // 滑动过期2分钟
		entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10); // 绝对过期10分钟

		if (head.IsMatch(result)) {
			var rawStr = result;
			result = aPartOfQQAPIResultRegex().Replace(head.Replace(result, ""), ""); // 处理数据
			result = System.Net.WebUtility.HtmlDecode(result).Replace(@"\", @"\\").Replace("\"", @"\""");

			entry.Value = result; // 写入内存缓存

			logger.LogDebug("已写入内存缓存：{}: {}", cacheKey, result);

			logger.LogDebug("成功获取 {}: {}，原始数据：{}", qq, result, rawStr);

			return http304.TrySet($"{false}|{result}") ? null : new() { Code = 0, Name = result, IsCache = false };
		}

		entry.Value = null; // 写入内存缓存

		logger.LogDebug("已写入内存缓存：{}: {}", cacheKey, null);

		logger.LogInformation("获取 {} 时未能匹配，原始数据：{}", qq, result);

		return http304.TrySet($"{true}|{null}") ? null : new() { Code = 2, Message = "无此 QQ 账号！", IsCache = false };
	} catch (HttpRequestException e) {
		logger.LogCritical("在 Get {} 时连接至QQ服务器过程中发生异常：{}", qq, e);
		return new() { Code = 3, Message = "无法连接 QQ API 服务器！" };
	} catch (TaskCanceledException e) {
		logger.LogCritical("在 Get {} 时连接至QQ服务器过程中发生异常：{}", qq, e);
		return new() { Code = 3, Message = "无法连接 QQ API 服务器！" };
	}
}



[GeneratedRegex("\",(\\-)?\\d{1,8}\\]\\}\\)")]
private static partial Regex aPartOfQQAPIResultRegex();
}