using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace API.Controllers {
	[ApiController]
	[Route("[controller]/{id}")]
	public class BiliVideoInfoController : ControllerBase {
		private readonly ILogger<BiliVideoInfoController> _logger;
		private readonly IHttpClientFactory _httpClientFactory;

		public BiliVideoInfoController(ILogger<BiliVideoInfoController> logger, IHttpClientFactory httpClientFactory) {
			_logger = logger;
			_httpClientFactory = httpClientFactory;
		}

		[HttpGet]
		public async Task<string> Get(string id) {
			if (string.IsNullOrWhiteSpace(id)) {
				return "{\"code\":2,\"message\":\"视频ID格式有误！\"}";
			}

			Regex p = new(@"^(av(\d+)|BV[A-Za-z0-9]+)$");
			var match = p.Match(id);
			if (match.Success) {
				string queryString = (id[..2] == "av") ? string.Concat("?aid=", id.AsSpan(2)) : ("?bvid=" + id);
				using var hc = _httpClientFactory.CreateClient("Timeout5s");
				hc.BaseAddress = new Uri("https://api.bilibili.com/x/web-interface/archive/stat");
				try {
					return await hc.GetStringAsync(queryString).ConfigureAwait(false);
				} catch {
					return "{\"code\":1,\"message\":\"无法连接哔哩哔哩服务器！\"}";
				}
			} else {
				return "{\"code\":2,\"message\":\"视频ID格式有误！\"}";
			}
		}
	}
}