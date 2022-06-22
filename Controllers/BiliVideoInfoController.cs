namespace API.Controllers {
	[ApiController]
	[Route(@"[controller]/{id:length(3,20):regex(^(av(\d+)|BV[[A-Za-z0-9]]+)$)}")]
	public class BiliVideoInfoController : ControllerBase {
		private readonly ILogger<BiliVideoInfoController> _logger;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IMemoryCache _memoryCache;

		public BiliVideoInfoController(ILogger<BiliVideoInfoController> logger, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache) {
			_logger = logger;
			_httpClientFactory = httpClientFactory;
			_memoryCache = memoryCache;
		}

		[HttpGet]
		public async Task<BiliVideoInfo> Get(string id) {
			Response.Headers.Remove("Cache-Control");
			Response.Headers.Add("Cache-Control", "private,max-age=600"); // 与滑动过期相匹配

			string cacheKey = "BiliVideoInfoAPI-" + id;
			if (_memoryCache.TryGetValue(cacheKey, out BiliVideoInfo info)) {
				_logger.LogDebug("BiliVideoInfo API 已命中内存缓存：{}: {}", cacheKey, JsonSerializer.Serialize(info));
				return info;
			}
            
			string queryString = (id[..2] == "av") ? string.Concat("?aid=", id.AsSpan(2)) : ("?bvid=" + id);
			using var hc = _httpClientFactory.CreateClient("Timeout5s");
			hc.BaseAddress = new Uri("https://api.bilibili.com/x/web-interface/archive/stat");
			try {
				info = await hc.GetFromJsonAsync<BiliVideoInfo>(queryString).ConfigureAwait(false);
                
				using var entry = _memoryCache.CreateEntry(cacheKey); // 写入内存缓存
				entry.Value = info;
				entry.SlidingExpiration = TimeSpan.FromMinutes(10); // 滑动过期10分钟
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(30); // 绝对过期1小时
				_logger.LogDebug("BiliVideoInfo API 已写入内存缓存：{}: {}", cacheKey, JsonSerializer.Serialize(info));

				return info;
			} catch (Exception e) {
				_logger.LogCritical("BiliVideoInfo API 在 Get {} 时连接至哔哩哔哩服务器过程中发生异常：{}", id, e);
				return new() { Code = 1, Message = "无法连接哔哩哔哩服务器！" };
			}
		}
	}
}