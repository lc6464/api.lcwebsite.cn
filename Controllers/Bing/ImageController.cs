using IO = System.IO;

namespace API.Controllers {
	[ApiController]
	[Route("Bing/[controller]")]
	[Route("Bing/[controller]/{id}")]
	public class ImageController : ControllerBase {
		private readonly ILogger<ImageController> _logger;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ImageController(ILogger<ImageController> logger, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment) {
			_logger = logger;
			_httpClientFactory = httpClientFactory;
			_webHostEnvironment = webHostEnvironment;
		}

		private string MapPath(string path) => Path.Combine(_webHostEnvironment.WebRootPath, path);

		private async Task<string> GetURLAndWriteFile(byte id, string filePath, HttpResponse response) {
			using var hc = _httpClientFactory.CreateClient("Timeout5s");
			hc.BaseAddress = new Uri("https://cn.bing.com/HPImageArchive.aspx?format=js&n=1&idx=" + id);
			try {
				var root = await hc.GetFromJsonAsync<BingAPIRoot>("").ConfigureAwait(false);
				var url = root.Images?[0].Url;
				if (string.IsNullOrWhiteSpace(url)) {
					return "未获取到 URL！";
                }
				try {
					IO.File.WriteAllText(filePath, url);
                } catch {
					response.Headers.Add("X-API-WriteCache-Error", "True");
				}
				return url;
			} catch (Exception e) {
				response.Headers.Add("X-API-Get-Error", e.Message);
				return "连接必应服务器失败！";
            }
			
		}
		

		[HttpGet]
		public async Task<string?> Get(byte id = 0) {
			if (id > 7) {
				Response.StatusCode = 400;
				return "HTTP 400 Bad Request\r\n传入的 id 有误！";
            }
			Response.Headers.Remove("Cache-Control"); // 缓存控制响应头

			var now = DateTime.Now; // 当前时间
			Response.Headers.Add("Cache-Control", "private,max-age=" + (int)(now.AddDays(1).Date - now).TotalSeconds); // 缓存时间
			string url, filePath = MapPath($"Cache/Bing/{now.AddDays(-id):yyyyMMdd}.txt"); // 声明变量及获取服务器缓存文件名

			if (IO.File.Exists(filePath)) { // 判断服务器缓存文件是否存在
				try { // 尝试读取缓存文件
					url = IO.File.ReadAllText(filePath); // 赋值
				} catch { // 若读取失败
					Response.Headers.Add("X-API-Read-CacheFile-Error", "True"); // X-API-读取-缓存文件-出错
					url = await GetURLAndWriteFile(id, filePath, Response).ConfigureAwait(false); // 重新获取并写入
					if (url[0] != '/') {
						return url;
                    }
				}
			} else { // 若不存在
				Response.Headers.Append("X-API-No-CacheFile", "True"); // X-API-无-缓存文件
				url = await GetURLAndWriteFile(id, filePath, Response).ConfigureAwait(false); // 获取并写入
				if (url[0] != '/') {
					return url;
				}
			}


			Response.Redirect("https://cn.bing.com" + url); // 重定向
			return "";
		}
	}
}