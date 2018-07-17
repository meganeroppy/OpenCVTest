using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGColorChange : MonoBehaviour {

    Camera myCamera;

	// Use this for initialization
	void Start () {
        myCamera = GetComponent<Camera>();

    }
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.Space))
        {
            Change();
        }
	}

    private void Change()
    {
        myCamera.backgroundColor = myCamera.backgroundColor == Color.white ? Color.black : Color.white;
    }
}
