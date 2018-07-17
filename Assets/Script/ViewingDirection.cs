using UnityEngine;
using System.Collections;


public class ViewingDirection : MonoBehaviour
{
	public static ViewingDirection instance;

	//目のメッシュ設定
	public Material eyeMatMain;

    [Tooltip("目のメッシュが左右で分かれているときに使用")]
	public Material eyeMatSub;

	private float dirValX = 0;
	private float dirValY = 0;
	private Vector2 eyeOffset;

	[SerializeField]
	Transform head;
    
	[SerializeField]
	Transform lookTarget;

	[SerializeField]
	bool log;

	const float EYE_X_MAX = 0.2f;
	const float EYE_Y_MAX = 0.1f;

	// Use this for initialization
	void Start () 
	{
		instance = this;
		//	eyeObjL = GameObject.Find("eye_L_old").GetComponent<MeshRenderer>();
		//	eyeObjR = GameObject.Find("eye_R_old").GetComponent<MeshRenderer>();
		eyeOffset = Vector2.zero;
	}

	void OnGUI() 
	{
		//スライダーと値
	//	GUILayout.BeginArea (new Rect(Screen.width - 250, 10, 400, 50));
	//	dirValX = GUILayout.HorizontalSlider ( dirValX, -1, 1, GUILayout.Width (150));
	//	GUILayout.Label ("Viewing direction  " + dirValX);
	//	GUILayout.Label ("Viewing direction  " + dirValY);
	//	GUILayout.EndArea ();

	//	SetEyeDir( dirValX, dirValY );
	}	

	/// <summary>
	/// 目の向きをセット
	/// X, Yそれぞれ-1~1
	/// </summary>
	void SetEyeDir( float dirX, float dirY )
	{
		eyeOffset = new Vector2(dirX * EYE_X_MAX , dirY * EYE_Y_MAX);
		eyeMatMain.SetTextureOffset("_MainTex", eyeOffset);
        if(eyeMatSub != null)
		eyeMatSub.SetTextureOffset("_MainTex", eyeOffset);
	}

	void Update()
	{
		SetEyeDir( GetCalculatedEyeDir(), 0);

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (shot)
            {
                var obj = Instantiate(shot, head.position, head.rotation).AddComponent<Rigidbody>();
                obj.AddForce(head.forward * 500f);
            }
        }
	}

	/// <summary>
	/// 目を向ける方向を計算する
	/// </summary>
	float GetCalculatedEyeDir()
	{
		// 頭の角度
		var headRot = head.rotation;

		// 自身からターゲットへの角度
		var diff = lookTarget.position - head.position;

		// 左右を判別するための軸を計算
		var axis = Vector3.Cross( head.forward, diff );

		// 正面からみた角度を計算
		var angle = Vector3.Angle( head.forward, diff ) * (axis.y < 0 ? -1 : 1) ;

		// 180で割る  正面=0 真右=0.5 真左=-0.5
		angle /= 180f;

		if( log )
			Debug.Log("顔正面からの角度 = " + angle.ToString() );

		// -1~1の値に変換
		var dirX = Mathf.InverseLerp( -0.5f, 0.5f, angle );
		dirX = Mathf.Lerp( -1, 1, dirX );

		if( log )
			Debug.Log("dirX = " + dirX.ToString() );


		return dirX;
	}

	[SerializeField]
	GameObject shot;
}
