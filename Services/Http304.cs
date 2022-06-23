using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace API.Services;
public class Http304 : IHttp304 {
	private readonly HttpRequest Request;
	private readonly HttpResponse Response;
	private readonly ILogger _logger;
	private readonly IHttpConnectionInfo _connection;
    private static readonly System.Reflection.Assembly _currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
	private static readonly Version _version = _currentAssembly.GetName().Version ?? new ();
	private static readonly DateTime _lastModified = File.GetLastWriteTime(_currentAssembly.Location);

	public Http304(HttpContext context, IHttpConnectionInfo connection, ILogger logger) {
		Request = context.Request;
		Response = context.Response;
		_connection = connection;
		_logger = logger;
	}

	public bool Set(bool withIP = false, string? value = "") {
		value ??= "";
		var ip = withIP ? _connection.RemoteAddress?.ToString() ?? "" : "";
		
		SHA256 sha256 = SHA256.Create();
		
		byte[] hash;
        
		string clientLM = Request.Headers["If-Modified-Since"],
			clientETag = Request.Headers["If-None-Match"];
		if (clientETag != null && clientLM == _lastModified.ToString("R") && clientETag.Length == 76) {
			clientETag = clientETag.Substring(3, 72);
			string clientSHA256 = string.Concat(clientETag.AsSpan(0, 32), clientETag.AsSpan(40)), clientSalt = clientETag.Substring(32, 8);
			hash = sha256.ComputeHash(utf8.GetBytes(ip + clientSalt + refresh));
			foreach (var h in hash) { sb.Append(h.ToString("X2")); }
			SHA256 = sb.ToString(); sb.Clear();
			if (clientSHA256 == SHA256) { sha256.Dispose(); Response.Clear(); Response.StatusCode = 304; }
		}
		string charList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()_+{}|:\"<>?-=[]\\;',./"; // Salt 中可包含的字符列表
		StringBuilder sb = new();
		for (int i = 0; i < 8; i++) sb.Append(charList[Random.Shared.Next(charList.Length)]);
		string salt = sb.ToString().ToUpper(), ETag;
		sb.Clear();
		hash = sha256.ComputeHash(utf8.GetBytes(ip + salt + refresh)); sha256.Dispose();
		foreach (var h in hash) { sb.Append(h.ToString("X2")); }
		SHA256 = sb.ToString(); sb.Clear();
		ETag = string.Concat(SHA256.AsSpan(0, 32), salt, SHA256.AsSpan(32));
        Response.Headers.Add("Last-Modified", LM); Response.Headers.Add("ETag", $"W/\"{ETag}\"");

        _logger.LogDebug("Http304 服务：已手动设置")
		// Set the 304 status code.
		_context.Response.StatusCode = (int)HttpStatusCode.NotModified;
	}
}