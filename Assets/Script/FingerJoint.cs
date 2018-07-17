using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerJoint : MonoBehaviour {
    public int fingerindex = 0;

    public Vector3 axis;
    public float angle;
    [HideInInspector]
    public Quaternion defaultRot;
}
