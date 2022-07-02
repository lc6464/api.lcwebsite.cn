namespace API.Controllers;
[ApiController]
[Route("Bing/Image/{id:int:range(0,7)?}")]
public class BingImageController : ControllerBase {
	private readonly ILogger<BingImageController> _logger;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IWebHostEnvironment _webHostEnvironment;
	private readonly IMemoryCache _memoryCache;


	private string? _filePath;

	private FileInfo? _fileInfo;

	/// <summary>
	/// 1 起始的行数
	/// </summary>
	private int _lines;

	private int _id;

	public BingImageController(ILogger<BingImageController> logger, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment, IMemoryCache memoryCache) {
		_logger = logger;
		_httpClientFactory = httpClientFactory;
		_webHostEnvironment = webHostEnvironment;
		_memoryCache = memoryCache;
	}

	private string MapPath(string path) => Path.Combine(_webHostEnvironment.WebRootPath, path);

	private async Task<BingAPIRoot?> GetBingAPI(int count) {
		using var hc = _httpClientFactory.CreateClient("Timeout5s");
		hc.BaseAddress = new("https://cn.bing.com/HPImageArchive.aspx");
		try {
			return await hc.GetFromJsonAsync<BingAPIRoot>($"?format=js&n={count}&idx={_id}").ConfigureAwait(false);
		} catch (Exception e) {
			_logger.LogCritical("在连接服务器时发生异常：{}", e);
		}
		return null;
	}

	/// <summary>
	/// 处理 GetBingAPI 返回的数据
	/// </summary>
	/// <param name="root">GetBingAPI 返回的数据</param>
	/// <param name="i">循环处理行时的中间变量</param>
	/// <param name="lines">要写入文件的内容</param>
	/// <param name="url">输出的 URL</param>
	/// <returns>如果处理成功，则为 <see cref="true"/>，否则为 <see cref="false"/></returns>
	private bool TryProcessData(BingAPIRoot? root, int i, ref List<string?> lines, out string url) {
		if (root == null) {
			url = "连接必应服务器失败！";
			return false;
		}

		if (root?.Images == null || root?.Images.Length == 0) {
			_logger.LogCritical("获取到的 URL 为空！");
			url = "未获取到 URL！";
			return false;
		}

		for (int a = _lines - 1, b = 0; a >= i + 1; a--, b++) // a >= _lines - n
			lines[a] = root?.Images[b].Url;

		try {
			using var writer = _fileInfo!.CreateText();

			foreach (string? one in lines) writer.WriteLine(one);
			
			writer.Flush();
			
			_logger.LogDebug("写入缓存文件成功。");
		} catch (Exception e) {
			_logger.LogError("写入缓存文件时发生异常：{}", e);
		}
		
		url = root?.Images[0].Url!;
		return true;
	}

	
	private bool TryProcessExistsLines(out bool exists, out List<string?> lines) {
		exists = _fileInfo!.Exists;
		lines = new();        
		if (exists) {
			try { // 读取缓存文件
				using var reader = _fileInfo.OpenText();
				string? line = reader.ReadLine();
				while (line != null) { // 把已有的行全部加入 List
					lines.Add(line);
					line = reader.ReadLine();
				}
				reader.BaseStream.Dispose();
				while (lines.Count < _lines) lines.Add(""); // 补足行
				return true;
			} catch (Exception e) {
				_logger.LogCritical("读取缓存文件时发生异常：{}", e);
				return false;
			}
		} else {
			do lines.Add(""); while (lines.Count < _lines); // 补足行
			return true;
		}
	}
	

	/// <summary>
	/// 从必应 API 获取数据并写入临时文件
	/// </summary>
	/// <returns>当前的 URL</returns>
	private async Task<string> GetAndProcessData() {

		if (TryProcessExistsLines(out _, out List<string?> lines)) {
			int i = _lines - 2; // (_lines - 1) - 1
			while (i >= _lines - 8 && i >= 0 && string.IsNullOrWhiteSpace(lines[i])) i--;
			// n = _lines - i - 1

			var root = await GetBingAPI(_lines - i - 1).ConfigureAwait(false);

			TryProcessData(root, i, ref lines, out var url);
			return url;
		} else {
			var root = await GetBingAPI(1).ConfigureAwait(false);
			if (root == null) return "连接必应服务器失败！";

			if (root?.Images == null || root?.Images.Length == 0) {
				_logger.LogCritical("获取到的 URL 为空！");
				return "未获取到 URL！";
			}
			return root?.Images[0].Url!;
		}
	}
	

	[HttpGet]
	public async Task<string?> Get(int id = 0) {
		TimeSpan cacheAge = DateTime.Today.AddDays(1) - DateTime.Now;

		_id = id;
		DateTime target = DateTime.Today.AddDays(-id); // 当前时间和目标日期
		_filePath = MapPath($"Cache/Bing/{target:yyyyMM}.txt"); // 声明变量及获取服务器缓存文件名
		_lines = target.Day;

		string cacheKey = "BingImageAPI-" + id;
		if (_memoryCache.TryGetValue(cacheKey, out string url)) { // 内存缓存
			_logger.LogDebug("已命中内存缓存：{}", cacheKey);
			_logger.LogDebug("输出的 URL：{}", url);
			Response.Redirect("https://cn.bing.com" + url); // 重定向
			return null;
		}

		_fileInfo = new(_filePath);
		if (_fileInfo.Exists) { // 判断服务器缓存文件是否存在
			try { // 尝试读取缓存文件
				using var reader = _fileInfo.OpenText();
				string? line = reader.ReadLine();
				for (int i = 1; i < _lines && line != null; i++) { // 读行
					line = reader.ReadLine();
				}
				reader.BaseStream.Dispose();
				if (string.IsNullOrWhiteSpace(line)) { // 指定行不存在或为空
					_logger.LogDebug("缓存文件 {} 对应行 {} 不存在或为空。", _filePath, _lines);
					url = await GetAndProcessData().ConfigureAwait(false); // 获取并写入
					if (url[0] != '/') { // 未获取到 URL
						Response.Headers.Add("Cache-Control", "private,max-age=10"); // 发生异常时缓存 10 秒
						return url;
					}
				} else {
					_logger.LogDebug("已命中缓存文件 {} 行 {}。", _filePath, _lines);
					url = line; // 赋值
				}
			} catch (Exception e) { // 若读取失败
				_logger.LogCritical("读取缓存文件时发生异常：{}", e);
				Response.Headers.Add("Cache-Control", "private,max-age=10"); // 发生异常时缓存 10 秒
				return "读取文件异常！";
			}
		} else { // 若不存在
			_logger.LogDebug("缓存文件 {} 不存在。", _filePath);
			url = await GetAndProcessData().ConfigureAwait(false); // 获取并写入
			if (url[0] != '/') { // 未获取到 URL
				Response.Headers.Add("Cache-Control", "private,max-age=10"); // 发生异常时缓存 10 秒
				return url;
			}
		}


		Response.Headers.Add("Cache-Control", "public,max-age=" + (int)cacheAge.TotalSeconds); // 缓存时间
		_memoryCache.Set(cacheKey, url, cacheAge);
		_logger.LogDebug("已将 {} 写入内存缓存 {} ，有效时间 {}。", url, cacheKey, cacheAge);
		_logger.LogDebug("输出的 URL：{}", url);
		Response.Redirect("https://cn.bing.com" + url); // 重定向
		return null;
	}
}