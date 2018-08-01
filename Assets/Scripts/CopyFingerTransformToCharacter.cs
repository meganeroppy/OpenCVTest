using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HI5;

public class CopyFingerTransformToCharacter : MonoBehaviour
{
    [SerializeField]
    HI5_VIVEInstance reference;

    [SerializeField]
    Transform[] copyTo;

	// Update is called once per frame
	void Update ()
    {
        if (!reference.IsValid || reference.HandBones == null) return;	

        for( int i=0; i< reference.HandBones.Length; ++i )
        {
            var b = reference.HandBones[i];

            if (copyTo.Length < i) break;
            else if (copyTo[i] == null) continue;

            copyTo[i].localPosition = b.localPosition;
            copyTo[i].localRotation = b.localRotation;
        }
	}
}
