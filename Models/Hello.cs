namespace API.Models;
public struct Hello {
	public Hello() {
	}

	public DateTime Time { get; } = DateTime.Now;
	public Version Version { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
	public string Text { get; init; } = "Welcome to API site of LC's Website.";
}