using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 高さのみターゲット（カメラ）と同じ高さ、それ以外コピー対象と同じ位置と回転をする
/// </summary>
public class EyeBase : MonoBehaviour {

	[SerializeField]
	Transform eyeBase;

	[SerializeField]
	Transform myCamera;

    [SerializeField]
    Vector3 rotDiff = Vector3.zero;

	Vector3 pos;

	// Update is called once per frame
	void Update () 
	{
		pos = eyeBase.position;
		transform.rotation = eyeBase.rotation * Quaternion.Euler( rotDiff );

		pos.y = myCamera.position.y; 
		transform.position = pos;
	}
}
