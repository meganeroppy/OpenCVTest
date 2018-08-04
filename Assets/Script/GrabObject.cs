using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof( SteamVR_TrackedObject))]
public class GrabObject : MonoBehaviour {

    CommentBoard current = null;

    SteamVR_TrackedObject controller;

    bool grabbedPrev = false;

    [SerializeField]
    float grabDistance = 0.1f;

    [SerializeField]
    Transform originParent;

	/// <summary>
	/// 前フレームの位置
	/// コメントを投げる処理に使用
	/// </summary>
	Vector3 prevPosition;

	[SerializeField]
	float throwForceRate = 100f;

	// Use this for initialization
	void Start () {
        controller = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
        var device = SteamVR_Controller.Input((int)controller.index);
        if (device == null) return;

//        var triggered = device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
//        var gripped = device.GetPress(SteamVR_Controller.ButtonMask.Grip);
        var grabbed = device.GetPress(SteamVR_Controller.ButtonMask.Trigger) || device.GetPress(SteamVR_Controller.ButtonMask.Grip);

        // 握ってない -> 握る
        if( !grabbedPrev && grabbed )
        {
            Grab();
        }

        // 握る -> 握ってない
        if (grabbedPrev && !grabbed)
        {
            Release();
        }

        grabbedPrev = grabbed;

		if( grabbed )
		{
			prevPosition = transform.position;
		}
    }

    void Grab()
    {
        if (current != null) return;

        var pool = CommentBoard.pool;
        float nearest = float.MaxValue;

        CommentBoard target = null;
        for( int i=0; i<pool.Count; ++i )
        {
            var obj = pool[i];
            if (!obj.gameObject.activeInHierarchy) continue;

            var distance = (transform.position - obj.transform.position).magnitude;
            if ( distance < grabDistance )
            {
                if( distance < nearest )
                {
                    target = obj;
                    nearest = distance;
                }
            }
        }

        if (target == null) return;

        current = target;

        current.grabbed = true;

        current.transform.SetParent(transform);
    }

    void Release()
    {
        if (current == null) return;

        current.transform.SetParent(originParent);

        current.grabbed = false;

		// 掴んで離したら物理挙動を有効にする

		// 投げる力を簡易的に計算
		var offset = transform.position - prevPosition;
		var dir = offset.normalized;
		var force = offset.magnitude * throwForceRate;

		current.EnablePhysics( dir * force );

        current = null;
    }
}
