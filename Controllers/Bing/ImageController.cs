using API.Models;
using System.Text.RegularExpressions;

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
        

		[HttpGet]
		public async Task Get(byte id = 0) {
			Response.Headers.Remove("Cache-Control"); // 缓存控制响应头

			var now = DateTime.Now; // 当前时间
			Response.Headers.Add("Cache-Control", "private,max-age=" + (int)(now.AddDays(1).Date - now).TotalSeconds); // 缓存时间
			string url, filePath = MapPath($"Cache/Bing/{now.AddDays(-id).ToString("yyyyMMdd")}.txt"); // 声明变量及获取服务器缓存文件名

			if (File.Exists(filePath)) { // 判断服务器缓存文件是否存在
				try { // 尝试读取缓存文件
					url = File.ReadAllText(filePath); // 赋值
					if (string.IsNullOrWhiteSpace(url)) { // 若缓存文件为空
						Response.AppendHeader("X-API-CacheFile-Empty", "True"); // X-API-缓存文件-空
						url = GetURLAndWriteFile(idx, filePath, Response); // 重新获取并写入
					}
				} catch { // 若读取失败
					Response.AppendHeader("X-API-Read-CacheFile-Error", "True"); // X-API-读取-缓存文件-出错
					url = GetURLAndWriteFile(idx, filePath, Response); // 重新获取并写入
				}
			} else { // 若不存在
				Response.AppendHeader("X-API-No-CacheFile", "True"); // X-API-无-缓存文件
				url = GetURLAndWriteFile(idx, filePath, Response); // 获取并写入
			}


			string type = Request.Unvalidated.QueryString["type"]; // 输出类型
			if (string.IsNullOrWhiteSpace(type)) { // 默认
				Response.Redirect(url); // 重定向
			} else if (type == "json") { // 返回 JSON
				Response.Write($"{{\"url\":\"{url}\",\"code\":0}}");
			} else if (type == "text") { // 返回纯文本
				Response.ContentType = "text/plain";
				Response.Write(url);
			} else if (type == "download") { // 下载并输出
				var hc = new HttpClient();
				try {
					var task = hc.GetByteArrayAsync(url); // 下载
					Response.ContentType = "image/jpeg";
					task.Wait(); // 等待下载完成
					Response.BinaryWrite(task.Result); // 输出
				} catch {
					Response.Headers.Remove("Cache-Control"); // 改缓存
					Response.AppendHeader("Cache-Control", "no-cache");
					Response.Write("{\"message\":\"下载图片失败！\",\"code\":3}");
				} finally {
					hc.Dispose();  // 释放资源
				}
			} else { // 默认
				Response.Redirect(url);
			}
		}
	}
}