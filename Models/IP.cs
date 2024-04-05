using System.Net;
using System.Net.Sockets;

namespace API.Models;
/// <summary>
/// 构造函数
/// </summary>
/// <param name="info">当前的 <see cref="IHttpConnectionInfo"/></param>
public readonly struct IP(IHttpConnectionInfo info) {
	/// <summary>
	/// 储存 IP 地址的中间变量
	/// </summary>
	private readonly IPAddress? _address = info.RemoteAddress;

	/// <summary>
	/// 读取 IP 地址
	/// </summary>
	public readonly string? Address => _address?.ToString();

	/// <summary>
	/// 读取地址族
	/// </summary>
	public readonly string? Family => _address?.AddressFamily switch {
		AddressFamily.InterNetwork => "IPv4",
		AddressFamily.InterNetworkV6 => "IPv6",
		_ => _address?.AddressFamily.ToString()
	};

	/// <summary>
	/// 当前连接所使用的 HTTP 协议
	/// </summary>
	public string Protocol => info.Protocol;
}