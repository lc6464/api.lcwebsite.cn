namespace API.Models;
public readonly struct BingAPIRoot {
	public BingImageData[]? Images { get; init; }
}

public readonly struct BingImageData {
	//public string Fullstartdate { get; init; }
	public string Url { get; init; }
	//public string Copyright { get; init; }
	//public string Copyrightlink { get; init; }
	//public string Title { get; init; }
}