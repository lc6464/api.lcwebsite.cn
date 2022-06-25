namespace API.Models;
public struct Hello {
	public Hello(HttpContext context) => IP = new(context.Connection.RemoteIpAddress, context.Request.Protocol);
	public Hello(IHttpConnectionInfo info) => IP = new(info);

	public DateTime Time { get; } = DateTime.Now;
	public Version Version { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
	public string Text { get; init; } = "Welcome to API site of LC's Website.";
	public IP IP { get; init; }
}