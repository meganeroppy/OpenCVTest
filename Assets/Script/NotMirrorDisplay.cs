using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotMirrorDisplay : MonoBehaviour {

    [SerializeField]
    bool key = false;

	// Use this for initialization
	void Start () {
        UnityEngine.XR.XRSettings.showDeviceView = key;
    }
}
