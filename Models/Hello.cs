using System.Reflection;

namespace API.Models;
public readonly struct Hello(IP ip, UserAgent ua) {
	private static readonly Assembly assembly = typeof(Hello).Assembly;

	public DateTime Time => DateTime.UtcNow;
	public Version Version => assembly.GetName().Version!;
	public string Copyright => ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute))!).Copyright;
	public string Text { get; init; } = "Welcome to API site of LC's Website.";
	public IP IP => ip;
	public UserAgent UA => ua;
}