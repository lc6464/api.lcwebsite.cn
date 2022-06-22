namespace API.Models {
	public struct BiliVideoInfo {
		/// <summary>
		/// 0：成功
		/// -400：请求错误
		/// -403：权限不足
		/// -404：无视频
		/// 62002：稿件不可见
		/// 62004：稿件审核中
		/// </summary>
		public int Code { get; set; }
		public string Message { get; set; }
		public BiliVideoInfoData? Data { get; set; }
	}
	public struct BiliVideoInfoData {

		/// <summary>
		/// AV 号
		/// </summary>
		public ulong Aid { get; set; }

		/// <summary>
		/// BV 号
		/// </summary>
		public string Bvid { get; set; }

		/// <summary>
		/// 播放量
		/// </summary>
		//public ulong View { get; set; }

		/// <summary>
		/// 弹幕量
		/// </summary>
		//public uint Danmaku { get; set; }

		/// <summary>
		/// 评论量
		/// </summary>
		//public uint Reply { get; set; }

		/// <summary>
		/// 收藏量
		/// </summary>
		//public uint Favorite { get; set; }

		/// <summary>
		/// 投币量
		/// </summary>
		//public uint Coin { get; set; }

		/// <summary>
		/// 分享量
		/// </summary>
		//public uint Share { get; set; }

		/// <summary>
		/// 获赞量
		/// </summary>
		//public uint Like { get; set; }

		/// <summary>
		/// 是否禁止转载
		/// </summary>
		//public byte No_reprint { get; set; }

		/// <summary>
		/// 视频类型 1：原创  2：转载
		/// </summary>
		//public byte Copyright { get; set; }
	}
}