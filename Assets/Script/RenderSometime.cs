using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一定間隔で１フレームだけ描画するカメラ
/// </summary>
public class RenderSometime : MonoBehaviour {

    Camera myCamera;

    [SerializeField]
    float renderInterval = 1f;

    float timer = 0;

	// Use this for initialization
	void Start ()
    {
        myCamera = GetComponent<Camera>();
        myCamera.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if ( timer >= renderInterval )
        {
            timer = 0;
            StartCoroutine(RenderOneFrame());
        }
	}

    /// <summary>
    /// １フレームだけカメラコンポーネントを有効にする
    /// </summary>
    IEnumerator RenderOneFrame()
    {
        myCamera.enabled = true;

        yield return null;

        myCamera.enabled = false;
    }
}
