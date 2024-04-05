namespace API.Models;
public readonly struct UserAgent {
	/// <summary>
	/// User-Agent 值
	/// </summary>
	public string? UA { get; init; }

	/// <summary>
	/// Sec-CH-UA 值
	/// </summary>
	public string? Sec_CH_UA { get; init; }

	/// <summary>
	/// Sec-CH-UA-Mobile 值
	/// </summary>
	public string? Sec_CH_UA_Mobile { get; init; }

	/// <summary>
	/// Sec-CH-UA-Platform 值
	/// </summary>
	public string? Sec_CH_UA_Platform { get; init; }
}