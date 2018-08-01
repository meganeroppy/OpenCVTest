using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerCamera : MonoBehaviour {

    [SerializeField]
    SteamVR_TrackedObject tracker = null;

    [SerializeField]
    Vector3 offsetPos;

    [SerializeField]
    Vector3 offsetRot;

    // Update is called once per frame
    void Update ()
    {
        if (tracker == null || !tracker.gameObject.activeInHierarchy) return;

        transform.SetPositionAndRotation(tracker.transform.position + offsetPos, tracker.transform.rotation * Quaternion.Euler(offsetRot));
	}
}
