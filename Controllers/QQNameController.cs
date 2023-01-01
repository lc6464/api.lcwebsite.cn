using System.Text;
using System.Text.RegularExpressions;

namespace API.Controllers;
[ApiController]
[Route(@"[controller]/{qq:length(5,13):regex(^\d{{5,13}}$)}")]
public partial class QQNameController : ControllerBase {
	private readonly ILogger<QQNameController> _logger;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IMemoryCache _memoryCache;
	private readonly IHttp304 _http304;

	public QQNameController(ILogger<QQNameController> logger, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IHttp304 http304) {
		_logger = logger;
		_httpClientFactory = httpClientFactory;
		_memoryCache = memoryCache;
		_http304 = http304;
	}

	[HttpGet]
	[ResponseCache(CacheProfileName = "Private10m")] // 与绝对过期时间匹配
	public async Task<QQName?> GetAsync(string qq) {

		var cacheKey = "QQNameAPI-" + qq;
		if (_memoryCache.TryGetValue(cacheKey, out string? qqName)) {
			_logger.LogDebug("已命中内存缓存：{}: {}", cacheKey, qqName);

			return _http304.TrySet($"{qqName == null}|{qqName}")
				? null
				: qqName == null ? new() { Code = 2, Message = "无此 QQ 账号！" } : new() { Code = 0, Name = qqName };
		}

		using var hc = _httpClientFactory.CreateClient("Timeout5s");
		hc.BaseAddress = new("https://r.qzone.qq.com/fcg-bin/cgi_get_portrait.fcg");
		try {
			var start = DateTime.Now;
			var data = await hc.GetByteArrayAsync("?uins=" + qq).ConfigureAwait(false);
			Response.Headers.Add("Server-Timing", $"g;desc=\"Get API\";dur={(DateTime.Now - start).TotalMilliseconds}"); // Server Timing API
			var result = Encoding.GetEncoding("GB18030").GetString(data); // Get 数据
			Regex head = new(@$"portraitCallBack\(\{{""{qq}"":\[""http://qlogo\d\d?\.store\.qq\.com/qzone/{qq}/{qq}/100"",((\-)?\d{{1,8}},){{5}}""");

			using var entry = _memoryCache.CreateEntry(cacheKey); // 创建内存缓存
			entry.SlidingExpiration = TimeSpan.FromMinutes(2); // 滑动过期2分钟
			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10); // 绝对过期10分钟

			if (head.IsMatch(result)) {
				var rawStr = result;
				result = aPartOfQQAPIResultRegex().Replace(head.Replace(result, ""), ""); // 处理数据
				result = System.Web.HttpUtility.HtmlDecode(result).Replace(@"\", @"\\").Replace("\"", @"\""");

				entry.Value = result; // 写入内存缓存

				_logger.LogDebug("已写入内存缓存：{}: {}", cacheKey, result);

				_logger.LogDebug("成功获取 {}: {}，原始数据：{}", qq, result, rawStr);

				return _http304.TrySet($"{false}|{result}") ? null : (new() { Code = 0, Name = result });
			}

			entry.Value = null; // 写入内存缓存

			_logger.LogDebug("已写入内存缓存：{}: {}", cacheKey, null);

			_logger.LogInformation("获取 {} 时未能匹配，原始数据：{}", qq, result);

			return _http304.TrySet($"{true}|{null}") ? null : (new() { Code = 2, Message = "无此 QQ 账号！" });
		} catch (Exception e) {
			_logger.LogCritical("在 Get {} 时连接至QQ服务器过程中发生异常：{}", qq, e);
			return new() { Code = 3, Message = "无法连接 QQ API 服务器！" };
		}
	}



	[GeneratedRegex("\",(\\-)?\\d{1,8}\\]\\}\\)")]
	private static partial Regex aPartOfQQAPIResultRegex();
}