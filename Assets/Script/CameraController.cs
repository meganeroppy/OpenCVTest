using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float spinSpeed = 1.0f;

	enum Movement
	{
		ReferInternalFaceTracking,
		ReferExternalFaceTracking,
		ReferObject,
		MousePosition,
		Input,
	}

	[SerializeField]
	Movement myMovement = Movement.MousePosition;

	[SerializeField]
	Transform referredObject;

	[SerializeField]
	float refferedObjectRate = 500f;

    Vector3 nowPos;
    Vector3 pos = Vector3.zero;
    Vector2 mouse = Vector2.zero;
	Vector2 input = Vector2.zero;
	Vector2 center = Vector2.one * 0.5f;

    [SerializeField]
    float moveSpeed = 1f;

    Vector3 targetPos = Vector3.zero;

	[SerializeField]
	Vector2 offsetLimit = Vector2.one * 1;

    /// <summary>
    /// 最低値 目算約200
    /// </summary>
    [SerializeField]
    Vector2 intPosMin = new Vector2(-700f, -100f);

    /// <summary>
    /// 最高値 目算約1000
    /// </summary>
    [SerializeField]
    Vector2 intPosMax = new Vector2(0f, 300f);


    /// <summary>
    /// 中央値
    /// </summary>
    Vector2 intPosMid = Vector2.zero;

    [SerializeField]
    float intDataRate = 400f;

    /// <summary>
    /// 最低値 目算約200
    /// </summary>
    [SerializeField]
	Vector2 extPosMin = new Vector2(200f, 100f);

	/// <summary>
	/// 最高値 目算約1000
	/// </summary>
	[SerializeField]
	Vector2 extPosMax = new Vector2( 1000f, 400f);

    [SerializeField]
    float extDataLerpRate = 400f;

    /// <summary>
    /// 中央値
    /// </summary>
    Vector2 extPosMid = Vector2.zero;

	[SerializeField]
	bool extPosXReverse = true;

    [SerializeField]
    bool extPosYReverse = true;

    [SerializeField]
	bool log = false;

    // Use this for initialization
    void Start()
    {
        // Canera get Start Position from Player
        nowPos = transform.position;

        if (target == null)
        {
            target = GameObject.FindWithTag("Player").transform;
            Debug.Log("player didn't setting. Auto search 'Player' tag.");
        }

        mouse.y = 0.5f; // start mouse y pos ,0.5f is half
		input = Vector2.one * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
		if( Input.GetKeyDown( KeyCode.R ) )
		{
			mouse = Vector2.zero;
			input = Vector2.zero;
		}

		float x = 0, y = 0;

		// OpenCVForUnityを使用したフェイストラッキング値を使用
		if( myMovement == Movement.ReferInternalFaceTracking )
		{
            // 中央値を計算
            intPosMid.x = intPosMin.x + (Mathf.Abs(intPosMax.x - intPosMin.x) * 0.5f);
            intPosMid.y = intPosMin.y + (Mathf.Abs(intPosMax.y - intPosMin.y) * 0.5f);

            // 0番目の値を使う
            CopyTransformSource source = null;
			if( CopyTransformSource.list != null && CopyTransformSource.list.Count >= 1 )
			{
				source = CopyTransformSource.list[0];
			}

			if( source == null ) return;

            var posX = source.transform.position.x;
            var posY = source.transform.position.y;

            if( log )
                Debug.Log(string.Format("fixed data = {0}, {1}", posX, posY));

            // x,yそれぞれを-0.5~0.5の値に補間する
            posX = Mathf.InverseLerp(intPosMin.x, intPosMax.x, posX) -.5f;
            posY = Mathf.InverseLerp(intPosMin.y, intPosMax.y, posY) -.5f;

            // 倍率適用し、0~1の範囲にする
            x = posX * intDataRate + 0.5f;
            y = posY * intDataRate + 0.5f;
        }

        // UDP経由で外部アプリケーションから受け取った値フェイストラッキング値を使用
        else if( myMovement == Movement.ReferExternalFaceTracking )
		{
            // 中央値を計算
            extPosMid.x = extPosMin.x + (Mathf.Abs(extPosMax.x - extPosMin.x) * 0.5f);
            extPosMid.y = extPosMin.y + (Mathf.Abs(extPosMax.y - extPosMin.y) * 0.5f);

            var posX = UDPParser.parsedData.x - extPosMid.x;
			if( extPosXReverse )
			{
				posX *= -1;
			}

            var posY = UDPParser.parsedData.y - extPosMid.y;
            if (extPosYReverse)
            {
                posY *= -1;
            }

            if (log)
                Debug.Log(string.Format("fixed data = {0}, {1}", posX, posY));

			x = Mathf.InverseLerp( -extDataLerpRate, extDataLerpRate, posX);
			y = Mathf.InverseLerp( -extDataLerpRate, extDataLerpRate, posY);
		}

		// 入力値を使用
		else if( myMovement == Movement.Input )
		{
			x = input.x += Input.GetAxis("Horizontal") * Time.deltaTime * spinSpeed;
			y = input.y += Input.GetAxis("Vertical") * Time.deltaTime * spinSpeed;
		}

		// マウスの位置を使用
		else if( myMovement == Movement.MousePosition )
		{
	        // Get MouseMove
	        mouse += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * spinSpeed;

			x = mouse.x;
			y = mouse.y;
		}

		// オブジェクトの位置を参照
		else
		{
			x = referredObject.position.x;
			y = referredObject.position.y;
		}

		// Clamp Y move
		y = Mathf.Clamp(y, -0.3f + 0.5f, 0.3f + 0.5f);

		x = Mathf.Clamp( x, center.x-offsetLimit.x, center.x+offsetLimit.x );
		y = Mathf.Clamp( y, center.y-offsetLimit.y, center.y+offsetLimit.y );

		if( log )
			Debug.Log( "( " +x.ToString() + ", "  + y.ToString() + " )" );

        // sphere coordinates
        pos.x = Mathf.Sin(y * Mathf.PI) * Mathf.Cos(x * Mathf.PI);
        pos.y = Mathf.Cos(y * Mathf.PI);
        pos.z = Mathf.Sin(y * Mathf.PI) * Mathf.Sin(x * Mathf.PI);

        // r and upper
        pos *= nowPos.z;

        pos.y += nowPos.y;
        // pos.x += nowPos.x; // if u need a formula,pls remove comment tag.

        // 目標位置をセット
        targetPos = pos + target.position;

        // ターゲット位置に移動しようとする
        {
            // 目標位置までの距離
            var distance = Vector3.Distance(transform.position, targetPos);

            // 目標座標が最大移動距離以下ならその位置まで移動
            var moveSpeedPerFrame = moveSpeed * Time.deltaTime;
            if (distance <= moveSpeedPerFrame)
            {
                transform.position = targetPos;
            }
            else
            {
              // 向きを正規化
                var direction = (targetPos - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }

            //	transform.position = pos;
            transform.LookAt(target.position);
        }
    }
}