using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRotate : MonoBehaviour {

    [SerializeField]
    float rotSpeed;


    // Update is called once per frame
    void Update() {
        var h = Input.GetAxis("Horizontal");

        transform.Rotate(Vector3.up * -h * rotSpeed * Time.deltaTime);
	}
}
