using API.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace API.Controllers {
	[ApiController]
	[Route("[controller]/{qq}")]
	public class QQNameController : ControllerBase {
		private readonly ILogger<QQNameController> _logger;
		private readonly IHttpClientFactory _httpClientFactory;

		public QQNameController(ILogger<QQNameController> logger, IHttpClientFactory httpClientFactory) {
			_logger = logger;
			_httpClientFactory = httpClientFactory;
		}

		[HttpGet]
		public async Task<QQName> Get(string qq, bool debug = false) {
			if (string.IsNullOrWhiteSpace(qq)) {
				return new() { Code = 4, Message = "QQ号为空！" };
			}
			Regex qqNum = new(@"^\d{5,13}$");
			if (!qqNum.IsMatch(qq)) {
				return new() { Code = 1, Message = "QQ 号应为5-13位数字！" };
			}
			using var hc = _httpClientFactory.CreateClient("Timeout5s");
			hc.BaseAddress = new Uri("https://r.qzone.qq.com/fcg-bin/cgi_get_portrait.fcg");
			try {
				var result = Encoding.GetEncoding("GB18030").GetString(await hc.GetByteArrayAsync("?uins=" + qq));
				if (debug) Response.Headers.Add("X-API-Return", result);
				Regex head = new(@$"portraitCallBack\(\{{""{qq}"":\[""http://qlogo\d\d?\.store\.qq\.com/qzone/{qq}/{qq}/100"",((\-)?\d{{1,8}},){{5}}""");
				if (head.IsMatch(result)) {
					result = Regex.Replace(Regex.Replace(result, head.ToString(), ""), @""",(\-)?\d{1,8}\]\}\)", "");
					result = System.Web.HttpUtility.HtmlDecode(result).Replace(@"\", @"\\").Replace("\"", @"\""");
					return new() { Code = 0, Name = result };
				} else {
					return new() { Code = 2, Message = "无此 QQ 账号！" };
				}
			} catch {
				return new() { Code = 3, Message = "无法连接 QQ API 服务器！"};
			}
		}
	}
}