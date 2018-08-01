using UnityEngine;
using System.Collections;


public class ViewingDirection : MonoBehaviour
{
	public static ViewingDirection instance;

	//目のメッシュ設定
	public Material eyeMatMain;

    [Tooltip("目のメッシュが左右で分かれているときに使用")]
	public Material eyeMatSub;

	private Vector2 eyeOffset;

	[SerializeField]
	Transform head;
    
    public Transform lookTarget;

    [SerializeField]
    Vector3 targetPosOffset = Vector3.zero;

	[SerializeField]
	bool log;

	const float EYE_X_MAX = 0.2f;
	const float EYE_Y_MAX_UP = 0.2f;
	const float EYE_Y_MAX_BOTTOM = 0.1f;

    [SerializeField]
    bool keepRotY = false;

	[SerializeField]
	bool vrmMode = false;

	[SerializeField]
	VRM.VRMLookAtHead vrmLookAt;

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
    void SetEyeDir(float dirX, float dirY)
    {
        if (keepRotY)
        {
            var offset = eyeMatMain.GetTextureOffset("_MainTex");
            eyeOffset = new Vector2(dirX * EYE_X_MAX, offset.y);
        }
        else
        {
			eyeOffset = new Vector2(dirX * EYE_X_MAX, dirY * ( dirY > 0 ? EYE_Y_MAX_BOTTOM : EYE_Y_MAX_UP));
        }
        eyeMatMain.SetTextureOffset("_MainTex", eyeOffset);

        if (eyeMatSub != null)
        {
            if (keepRotY)
            {
                var offset = eyeMatSub.GetTextureOffset("_MainTex");
                eyeOffset = new Vector2(dirX * EYE_X_MAX, offset.y);
            }
            else
            {
				eyeOffset = new Vector2(dirX * EYE_X_MAX, dirY * ( dirY > 0 ? EYE_Y_MAX_BOTTOM : EYE_Y_MAX_UP));
            }

            eyeMatSub.SetTextureOffset("_MainTex", eyeOffset);
        }
    }

	void Update()
	{
		if( vrmMode && vrmLookAt != null )
		{
			GetCalculatedEyeDirFromVRM();
			SetEyeDir( yawAndPitch.x, yawAndPitch.y );
		}
		else
		{
        	SetEyeDir(GetCalculatedEyeDir(), GetCalculatedEyeDirV());
		}

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
		var diff = (lookTarget.position + targetPosOffset) - head.position;

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

    /// <summary>
    /// 目を向ける方向を計算する
    /// </summary>
    float GetCalculatedEyeDirV()
    {
        // 頭の角度
        var headRot = head.rotation;

        // 自身からターゲットへの角度
        var diff = (lookTarget.position + targetPosOffset) - head.position;

        // 左右を判別するための軸を計算
        var axis = Vector3.Cross(head.forward, diff);

        // 正面からみた角度を計算
        var angle = Vector3.Angle(head.forward, diff) * (axis.x < 0 ? -1 : 1);

        // 180で割る  正面=0 真右=0.5 真左=-0.5
        angle /= 180f;

        if (log)
            Debug.Log("顔正面からの角度 = " + angle.ToString());

        // -1~1の値に変換
        var dirY = Mathf.InverseLerp(-0.5f, 0.5f, angle);
        dirY = Mathf.Lerp(-1, 1, dirY);

        if (log)
            Debug.Log("dirY = " + dirY.ToString());


        return dirY;
    }

	Vector2 yawAndPitch = Vector2.zero;

	[SerializeField]
	float maxYaw = 50f;
	[SerializeField]
	float maxPitch = 50f;

	void GetCalculatedEyeDirFromVRM()
	{
        // ターゲットカメラを更新
        //var gm = EggGameManager.instance;
        //if (gm == null || gm.CameraList == null || gm.CameraList.Count <= 0 || gm.CameraList.Count < gm.CurrentCameraIndex) return;
        //vrmLookAt.Target = gm.CameraList[gm.CurrentCameraIndex].transform;

		var yaw = vrmLookAt.Yaw;

		// 設定された最大値に納める
		yaw = Mathf.Clamp( yaw, -maxYaw, maxYaw );

		yaw = Mathf.InverseLerp( -maxYaw, maxYaw, yaw );
		yaw = Mathf.Lerp( -1, 1, yaw );
		yawAndPitch.x = yaw;

		var pitch = vrmLookAt.Pitch;

		// 設定された最大値に納める
		pitch = Mathf.Clamp( pitch, -maxPitch, maxPitch );

		pitch = Mathf.InverseLerp( -maxPitch, maxPitch, pitch );
		pitch = Mathf.Lerp( -1, 1, pitch );

		// picchは反転する
		yawAndPitch.y = -pitch;

//		pitchAndYaw.x = vrmLookAt.Yaw;
//		pitchAndYaw.y = vrmLookAt.Pitch;

		if (log)			
		Debug.Log( string.Format("pitch = {0} yaw = {1}", yawAndPitch.x, yawAndPitch.y) );
	}

    private void OnDestroy()
    {
        eyeMatMain.SetTextureOffset("_MainTex", Vector2.zero);
        if (eyeMatSub != null)
            eyeMatSub.SetTextureOffset("_MainTex", Vector2.zero);
    }

    [SerializeField]
	GameObject shot;
}
