using System.Reflection;

namespace API.Models;
public struct Hello {
	private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
	public Hello(HttpContext context) => IP = new(context.Connection.RemoteIpAddress, context.Request.Protocol);

	public Hello(IHttpConnectionInfo info) => IP = new(info);

	public DateTime Time { get; } = DateTime.Now;
	public Version Version { get; } = assembly.GetName().Version!;
	public string Copyright { get; } = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute))!).Copyright;
	public string Text { get; init; } = "Welcome to API site of LC's Website.";
	public IP IP { get; init; }
}