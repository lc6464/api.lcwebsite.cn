using System.Text;
using System.Text.RegularExpressions;

namespace API.Controllers {
	[ApiController]
	[Route(@"[controller]/{qq:length(5,13):regex(^\d{{5,13}}$)}")]
	public class QQNameController : ControllerBase {
		private readonly ILogger<QQNameController> _logger;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IMemoryCache _memoryCache;

		public QQNameController(ILogger<QQNameController> logger, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache) {
			_logger = logger;
			_httpClientFactory = httpClientFactory;
			_memoryCache = memoryCache;
		}

		[HttpGet]
		public async Task<QQName> Get(string qq) {
			Response.Headers.Remove("Cache-Control");
			Response.Headers.Add("Cache-Control", "private,max-age=30");
			string cacheKey = "QQNameAPI-" + qq;
			if (_memoryCache.TryGetValue(cacheKey, out string? qqName)) {
				_logger.LogDebug("QQ昵称 API 已命中内存缓存：{}: {}", cacheKey, qqName);
				return qqName == null ? new() { Code = 2, Message = "无此 QQ 账号！" } : new() { Code = 0, Name = qqName };
			}

			using var hc = _httpClientFactory.CreateClient("Timeout5s");
			hc.BaseAddress = new Uri("https://r.qzone.qq.com/fcg-bin/cgi_get_portrait.fcg");
			try {
				var result = Encoding.GetEncoding("GB18030").GetString(await hc.GetByteArrayAsync("?uins=" + qq).ConfigureAwait(false));
				Regex head = new(@$"portraitCallBack\(\{{""{qq}"":\[""http://qlogo\d\d?\.store\.qq\.com/qzone/{qq}/{qq}/100"",((\-)?\d{{1,8}},){{5}}""");
				if (head.IsMatch(result)) {
					string rawStr = result;
					result = Regex.Replace(Regex.Replace(result, head.ToString(), ""), @""",(\-)?\d{1,8}\]\}\)", "");
					result = System.Web.HttpUtility.HtmlDecode(result).Replace(@"\", @"\\").Replace("\"", @"\""");

					using var entry = _memoryCache.CreateEntry(cacheKey); // 写入内存缓存
					entry.Value = result;
					entry.SlidingExpiration = TimeSpan.FromMinutes(2); // 滑动过期2分钟
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10); // 绝对过期10分钟
					_logger.LogDebug("QQ昵称 API 已写入内存缓存：{}: {}", cacheKey, result);

					_logger.LogDebug("QQ昵称 API 成功匹配 {}: {}，原始数据：{}", qq, result, rawStr);
					return new() { Code = 0, Name = result };
				} else {
					using var entry = _memoryCache.CreateEntry(cacheKey); // 写入内存缓存
					entry.Value = null;
					entry.SlidingExpiration = TimeSpan.FromMinutes(2); // 滑动过期2分钟
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10); // 绝对过期10分钟
					_logger.LogDebug("QQ昵称 API 已写入内存缓存：{}: {}", cacheKey, null);

					_logger.LogInformation("QQ昵称 API 获取 {} 时未能匹配，原始数据：{}", qq, result);
					return new() { Code = 2, Message = "无此 QQ 账号！" };
				}
			} catch (Exception e) {
				_logger.LogCritical("QQ昵称 API 在 Get {} 时连接至QQ服务器过程中发生异常：{}", qq, e);
				return new() { Code = 3, Message = "无法连接 QQ API 服务器！"};
			}
		}
	}
}