using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAssignTrackerIndex : MonoBehaviour {

    SteamVR_TrackedObject[] trackerObjects;

    Dictionary<string, int> keyValuePair;

	// Use this for initialization
	void Start() {
        trackerObjects = transform.GetComponentsInChildren<SteamVR_TrackedObject>();

        keyValuePair = new Dictionary<string, int>();

        foreach( SteamVR_TrackedObject o in trackerObjects)
        {
            keyValuePair.Add(o.name, (int)o.index);

            Debug.Log(o.name + "のインデックスは" + o.index.ToString());
        }
	}

    // Update is called once per frame
    void Update () {
		if( Input.GetKeyDown(KeyCode.Space))
        {
            Exec();
        }
	}

    void Exec()
    {
        foreach (SteamVR_TrackedObject o in trackerObjects)
        {
            if( !keyValuePair.ContainsKey(o.name))
            {
                Debug.LogWarning( o.name + "が未登録" );
                continue;
            }
            var index = keyValuePair[ o.name ] ;

            var prev = o.index;

         //   o.index = (SteamVR_TrackedObject.EIndex)index;
            o.SetDeviceIndex(index);

            Debug.Log(o.name + "のインデックス更新 [ " + prev.ToString() + " -> " + o.index.ToString() + " ]" );
        }
    }
}
