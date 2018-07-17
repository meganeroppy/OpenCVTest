using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateReset : MonoBehaviour
{

    Vector3 defaultPos;

    Quaternion defaultRot;

    // Use this for initialization
    void Awake
        ()
    {
        defaultPos = transform.position;
        defaultRot = transform.rotation;
        Debug.Log(string.Format("defPos = {0} defRot = {1}", transform.position, transform.rotation));

    }

    public void Reset()
    {
        Debug.Log("Reset");
        transform.position = defaultPos;
        transform.rotation = defaultRot;


        Debug.Log(string.Format("pos = {0} rot = {1}", transform.position, transform.rotation));
    }
}
