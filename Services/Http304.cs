using System.Net;
using System.Text;

namespace API.Services;
public class Http304 : Interfaces.IHttp304 {
	private readonly HttpRequest Request;
	private readonly HttpResponse Response;
	private readonly ILogger _logger;
	private readonly Version _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version ?? new ();

	public Http304(HttpContext context, ILogger logger) {
		Request = context.Request;
		Response = context.Response;
		_logger = logger;
	}

	public bool Set(bool withIP = false) {
		string charList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()_+{}|:\"<>?-=[]\\;',./"; // Salt 中可包含的字符列表
		int year = DateTime.Now.Year, month = DateTime.Now.Month;
		string ip = withIP ? IP.GetIP(Request).ToString() : ""; StringBuilder sb = new();
		for (int i = 0; i < 8; i++) { sb.Append(charList[Random.Shared.Next(charList.Length)]); }
		string salt = sb.ToString().ToUpper(), ETag, SHA256;
		sb.Clear(); SHA256Managed sha256 = new(); byte[] hash;
		NameValueCollection headers = Request.Headers; string clientLM = headers["If-Modified-Since"], clientETag = headers["If-None-Match"];
		UTF8Encoding utf8 = new(false, true);
		if (clientETag != null && clientLM == LM && clientETag.Length == 76) {
			clientETag = clientETag.Substring(3, 72);
			string clientSHA256 = string.Concat(clientETag.AsSpan(0, 32), clientETag.AsSpan(40)), clientSalt = clientETag.Substring(32, 8);
			hash = sha256.ComputeHash(utf8.GetBytes(year + month + ip + clientSalt + refresh));
			foreach (var h in hash) { sb.Append(h.ToString("X2")); }
			SHA256 = sb.ToString(); sb.Clear();
			if (clientSHA256 == SHA256) { sha256.Dispose(); Response.Clear(); Response.StatusCode = 304; }
		}
		hash = sha256.ComputeHash(utf8.GetBytes(year + month + ip + salt + refresh)); sha256.Dispose();
		foreach (var h in hash) { sb.Append(h.ToString("X2")); }
		SHA256 = sb.ToString(); sb.Clear();
		ETag = string.Concat(SHA256.AsSpan(0, 32), salt, SHA256.AsSpan(32));
		Response.AddHeader("Last-Modified", LM); Response.AddHeader("ETag", "W/\"" + ETag + "\"");
        
		_logger.LogDebug("Http304 服务：已手动设置")
		// Set the 304 status code.
		_context.Response.StatusCode = (int)HttpStatusCode.NotModified;
	}
}