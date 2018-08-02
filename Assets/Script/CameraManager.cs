using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class CameraManager : MonoBehaviour {

    [SerializeField]
    Camera[] cameraList;

    int currentCameraIndex = 0;

    [SerializeField]
    VRMLookAtHead lookAtHead;

	// Use this for initialization
	void Start () {
        SwitchCamera();
	}

	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.C) )
        {
            currentCameraIndex = (currentCameraIndex + 1) % cameraList.Length;
            SwitchCamera();
        }
	}

    void SwitchCamera()
    {
        for( int i=0; i < cameraList.Length; ++i)
        {
            bool flag = i == currentCameraIndex;
            cameraList[i].enabled = flag;
            if( flag )
            {
                lookAtHead.Target = cameraList[i].transform;
            }
        }
    }
}
