namespace API.Models;
public readonly struct UserAgent(IHeaderDictionary headers) {
	/// <summary>
	/// User-Agent 值
	/// </summary>
	[JsonPropertyName("user-agent")]
	public string? UA { get; init; } = headers.UserAgent;

	/// <summary>
	/// Sec-CH-UA 值
	/// </summary>
	[JsonPropertyName("sec-ch-ua")]
	public string? Sec_CH_UA { get; init; } = headers["Sec-CH-UA"];

	/// <summary>
	/// Sec-CH-UA-Mobile 值
	/// </summary>
	[JsonPropertyName("sec-ch-ua-mobile")]
	public string? Sec_CH_UA_Mobile { get; init; } = headers["Sec-CH-UA-Mobile"];

	/// <summary>
	/// Sec-CH-UA-Platform 值
	/// </summary>
	[JsonPropertyName("sec-ch-ua-platform")]
	public string? Sec_CH_UA_Platform { get; init; } = headers["Sec-CH-UA-Platform"];
}