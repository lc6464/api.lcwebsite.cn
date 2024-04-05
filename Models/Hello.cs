using System.Reflection;

namespace API.Models;
public readonly struct Hello(IHttpConnectionInfo info) {
	private static readonly Assembly assembly = typeof(Hello).Assembly;

	public DateTime Time => DateTime.Now;
	public Version Version => assembly.GetName().Version!;
	public string Copyright => ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute))!).Copyright;
	public string Text { get; init; } = "Welcome to API site of LC's Website.";
	public IP IP => new(info);
}