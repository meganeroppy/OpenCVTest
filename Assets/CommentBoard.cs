using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EggSystem;
using YouTubeLive;

public class CommentBoard : MonoBehaviour 
{
	public static List<CommentBoard> pool;

	[SerializeField]
	TextMesh name = null;
	[SerializeField]
	TextMesh comment = null;
	[SerializeField]
	SpriteRenderer image;

	public void Set(  Chat.Msg msg  )
	{
		name.text = msg.name;
		comment.text = msg.text;	

		if( !string.IsNullOrEmpty( msg.img ) )
		{
			StartCoroutine( LoadImage( msg.img ) ); 
		}
	}

	Rect iconRect = new Rect(0,0,88,88);

	IEnumerator LoadImage( string url )
	{
		WWW www = new WWW(url);

		// 画像ダウンロード完了を待機
		yield return www;

		// webサーバから取得した画像をRaw Imagで表示する
		var texture = www.textureNonReadable;

		image.sprite = Sprite.Create( texture, iconRect, Vector2.zero);

//		RawImage rawImage = GetComponent<RawImage>();
//		rawImage.texture = www.textureNonReadable;
	}
}
