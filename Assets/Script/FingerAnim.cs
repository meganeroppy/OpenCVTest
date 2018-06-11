using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerAnim : MonoBehaviour {

    FingerJoint[] fingers;

    [SerializeField]
    float animSpeed = 1f;

    bool current = false;

    private void Start()
    {
        fingers = transform.GetComponentsInChildren<FingerJoint>();

        foreach( FingerJoint f in fingers)
        {
            f.defaultRot = f.transform.localRotation;
        }
    }

    // Update is called once per frame
    void Update () {
	if( Input.GetKeyDown( KeyCode.H ))
        {
            Hold();
        }
	}

    void Hold()
    {
        current = !current;
        StartCoroutine(ExecHold( current ));
    }

    IEnumerator ExecHold( bool key )
    {
        float progress = 0;
        Quaternion rot;
        if( key )
        {
            while (progress < 1)
            {
                progress += animSpeed * Time.deltaTime;
                foreach (FingerJoint f in fingers)
                {
                    float angleTo = f.angle * progress;  

                    f.transform.localRotation = f.defaultRot * Quaternion.Euler(f.axis * angleTo);
                }

                yield return null;
            }
        }
        else
        {
            while (progress < 1)
            {
                progress += animSpeed * Time.deltaTime;
                foreach (FingerJoint f in fingers)
                {
                    float angleTo = f.angle * (1f - progress);

                    f.transform.localRotation = f.defaultRot * Quaternion.Euler(f.axis * angleTo);
                }

                yield return null;
            }



        //    foreach (FingerJoint f in fingers)
        //    {
        //        f.transform.localRotation = f.defaultRot;
        //    }
        }

    //    yield break;
    }
}
