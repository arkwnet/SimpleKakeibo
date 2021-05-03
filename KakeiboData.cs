using System;

namespace SimpleKakeibo
{
	class KakeiboData
	{
		int kakeiboMode;
		DateTime kakeiboDateTime;
		int kakeiboMoney;
		string kakeiboMemo;
		public KakeiboData(int mode, DateTime date, int money, string memo)
		{
			kakeiboMode = mode;
			kakeiboDateTime = date;
			kakeiboMoney = money;
			kakeiboMemo = memo;
		}
		public int GetKakeiboMode() { return kakeiboMode; }
		public DateTime GetKakeiboDateTime() { return kakeiboDateTime; }
		public int GetKakeiboMoney() { return kakeiboMoney; }
		public string GetKakeiboMemo() { return kakeiboMemo; }
	}
}
