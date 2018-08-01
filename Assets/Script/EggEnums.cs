using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EggSystem
{
    public enum Emote
    {
        Happy,
        Angry,
        Sad,
        Joy
    }

    /// <summary>
    /// 追々Emoteと統合してもよいかも
    /// </summary>
    public enum Facial
    {
        Normal,
        Angry,
        Sad,
        Smile,
    }

    public enum EyeType
    {
        A,
        B,
    }

    public enum MouthType
    {
        A,
        B,
    }

    public enum ContentType
    {
        Prin,
        Gumi,
        Kyaramel,
        Count
    }

	/// <summary>
	/// ライブ配信中の視聴者コメント情報
	/// </summary>
	public struct UserInfo
	{
		public string name;
		public string comment;
	}
}

