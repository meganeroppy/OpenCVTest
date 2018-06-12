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
    void Update() {
        if( Input.GetKeyDown(KeyCode.R) )
        {
            rot = transform.rotation.eulerAngles;
            rot.y = 180f - source.transform.localRotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(rot);
        }

    //    var h = Input.GetAxis("Horizontal");

    //    transform.Rotate(Vector3.up * -h * rotSpeed * Time.deltaTime);
	}
}
