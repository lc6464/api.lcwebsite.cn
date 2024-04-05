namespace API.Models;
public readonly struct QQName {
	public int Code { get; init; }
	public string? Name { get; init; }
	public string? Message { get; init; }
	public bool? IsCache { get; init; }
}