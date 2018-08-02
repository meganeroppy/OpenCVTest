using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EggSystem;
using YouTubeLive;
using TMPro;

public class CommentBoard : MonoBehaviour 
{
	public static List<CommentBoard> pool;

	[SerializeField]
    TextMeshPro name = null;
	[SerializeField]
    TextMeshPro comment = null;
	[SerializeField]
	SpriteRenderer image;

    Rigidbody rb;

    float lifeTime = 60f;
    float timer = 0;

    [SerializeField]
    GameObject popEffect;
    [SerializeField]
    GameObject delEffect;

    [SerializeField]
    float effectScale = 0.5f;

    bool setOnce = false;

    public bool grabbed = false;

    void Awake()
    {
        if( rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    public void Set(  Chat.Msg msg, float lifeTime=0 )
	{
		name.text = msg.name;
		comment.text = msg.text;	

		if( !string.IsNullOrEmpty( msg.img ) )
		{
			StartCoroutine( LoadImage( msg.img ) ); 
		}

        this.lifeTime = lifeTime;
        timer = 0;

        var efs = Instantiate(popEffect);
        efs.transform.localScale = Vector3.one * effectScale;
        efs.transform.position = transform.position;

        setOnce = true;
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

    private void Update()
    {
        // 掴まれている間はタイマーの更新を停止
        if (grabbed) return;

        timer += Time.deltaTime;
        if( timer >= lifeTime )
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (setOnce && !quited)
        {
            var efs = Instantiate(delEffect);
            efs.transform.localScale = Vector3.one * effectScale;
            efs.transform.position = transform.position;
        }

        setOnce = false;
    }

    bool quited = false;

    private void OnApplicationQuit()
    {
        quited = true;
    }
}
