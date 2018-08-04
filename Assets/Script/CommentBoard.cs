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
//	public Rigidbody Rb{ get{ return rb; } }

	BoxCollider bc;
//	public BoxCollider Bc{ get{ return bc; } }

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
		if( bc == null)
		{
			bc = GetComponent<BoxCollider>();
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
			if( rb.constraints == RigidbodyConstraints.FreezeAll )
			{
				EnablePhysics();
			}
        }
    }

	void OnEnable()
	{
		rb.constraints = RigidbodyConstraints.FreezeAll;
		bc.enabled = false;
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

	/// <summary>
	/// 物理挙動を有効にする
	/// </summary>
	public void EnablePhysics(){ EnablePhysics(Vector3.zero); }

	public void EnablePhysics( Vector3 addForce )
	{
		rb.constraints = RigidbodyConstraints.None;
		rb.AddForce(addForce);
		bc.enabled = true;
	}

	void OnCollisionEnter( Collision col )
	{
		gameObject.SetActive(false);
	}
}
