namespace API.Models;
public readonly struct BiliVideoInfo {
	/// <summary>
	/// 0：成功
	/// -400：请求错误
	/// -403：权限不足
	/// -404：无视频
	/// 62002：稿件不可见
	/// 62004：稿件审核中
	/// </summary>
	public int Code { get; init; }
	public string? Message { get; init; }
	public BiliVideoInfoData? Data { get; init; }
}

public readonly struct BiliVideoInfoData {
	/// <summary>
	/// AV 号
	/// </summary>
	public ulong Aid { get; init; }

	/// <summary>
	/// BV 号
	/// </summary>
	public string Bvid { get; init; }

	/// <summary>
	/// 播放量
	/// </summary>
	//public ulong View { get; init; }

	/// <summary>
	/// 弹幕量
	/// </summary>
	//public uint Danmaku { get; init; }

	/// <summary>
	/// 评论量
	/// </summary>
	//public uint Reply { get; init; }

	/// <summary>
	/// 收藏量
	/// </summary>
	//public uint Favorite { get; init; }

	/// <summary>
	/// 投币量
	/// </summary>
	//public uint Coin { get; init; }

	/// <summary>
	/// 分享量
	/// </summary>
	//public uint Share { get; init; }

	/// <summary>
	/// 获赞量
	/// </summary>
	//public uint Like { get; init; }

	/// <summary>
	/// 是否禁止转载
	/// </summary>
	//public byte No_reprint { get; init; }

	/// <summary>
	/// 视频类型 1：原创  2：转载
	/// </summary>
	//public byte Copyright { get; init; }
}