namespace API.Models {
	public struct BiliVideoInfo {
		/// <summary>
		/// 0���ɹ�
		/// -400���������
		/// -403��Ȩ�޲���
		/// -404������Ƶ
		/// 62002��������ɼ�
		/// 62004����������
		/// </summary>
		public int Code { get; set; }
		public string Message { get; set; }
		public BiliVideoInfoData? Data { get; set; }
	}
	public struct BiliVideoInfoData {

		/// <summary>
		/// AV ��
		/// </summary>
		public ulong Aid { get; set; }

		/// <summary>
		/// BV ��
		/// </summary>
		public string Bvid { get; set; }

		/// <summary>
		/// ������
		/// </summary>
		public ulong View { get; set; }

		/// <summary>
		/// ��Ļ��
		/// </summary>
		public uint Danmaku { get; set; }

		/// <summary>
		/// ������
		/// </summary>
		public uint Reply { get; set; }

		/// <summary>
		/// �ղ���
		/// </summary>
		public uint Favorite { get; set; }

		/// <summary>
		/// Ͷ����
		/// </summary>
		public uint Coin { get; set; }

		/// <summary>
		/// ������
		/// </summary>
		public uint Share { get; set; }

		/// <summary>
		/// ������
		/// </summary>
		public uint Like { get; set; }

		/// <summary>
		/// �Ƿ��ֹת��
		/// </summary>
		public byte No_reprint { get; set; }

		/// <summary>
		/// ��Ƶ���� 1��ԭ��  2��ת��
		/// </summary>
		public byte Copyright { get; set; }
	}
}