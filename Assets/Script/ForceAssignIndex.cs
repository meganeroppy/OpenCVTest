using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAssignIndex : MonoBehaviour {

    [SerializeField]
    int definedId = 0;

    SteamVR_TrackedObject obj;

	// Use this for initialization
	void Start () {
        obj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown( KeyCode.K ))
        {
            if( obj == null ) obj = GetComponent<SteamVR_TrackedObject>();

            obj.SetDeviceIndex(definedId);
        }
	}
}
