using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRotate : MonoBehaviour {

    [SerializeField]
    float rotSpeed;

    [SerializeField]
    Transform source;

    Vector3 rot;

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.R) )
        {
            Recenter();
        }

        var trackedObject = GetComponent<SteamVR_TrackedObject>();
        if (trackedObject == null) return;
        var device = SteamVR_Controller.Input((int)trackedObject.index);
        if (device == null) return;

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Recenter();
        }

        //    var h = Input.GetAxis("Horizontal");

        //    transform.Rotate(Vector3.up * -h * rotSpeed * Time.deltaTime);
    }

    void Recenter()
    {
        rot = transform.rotation.eulerAngles;
        rot.y = 180f - source.transform.localRotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(rot);
    }
}
