using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerAnim : MonoBehaviour {

    FingerJoint[] fingers;

    [SerializeField]
    float animSpeed = 1f;

    [SerializeField]
    SteamVR_TrackedObject controller;

    bool current = false;

    private void Start()
    {
        fingers = transform.GetComponentsInChildren<FingerJoint>();

        foreach( FingerJoint f in fingers)
        {
            f.defaultRot = f.transform.localRotation;
        }
    }

    bool holding = false;

    float currentHoldPercent = 0;

    // Update is called once per frame
    void Update () {
        var device = SteamVR_Controller.Input((int)controller.index);
        if (device == null) return;

        var triggered = device.GetPress(SteamVR_Controller.ButtonMask.Trigger);

        var holdParcentPrev = currentHoldPercent;

        /*
        if ( triggered && !holding )
        {
            Hold(holding = true);
        }
        else if( !triggered && holding  )
        {
            Hold(holding = false);
        }
        */

        if (triggered && currentHoldPercent < 1f)
        {
            currentHoldPercent += animSpeed * Time.deltaTime;
        }
        else if( !triggered && currentHoldPercent > 0 )
        {
            currentHoldPercent -= animSpeed * Time.deltaTime;
        }

        if ( Mathf.Abs( currentHoldPercent - holdParcentPrev) > 0.001f  )
        {
            UpdateHold();
        }

        if ( Input.GetKeyDown( KeyCode.H ))
        {
            current = !current;
            Hold(current );
        }
	}

    Coroutine holdCo = null;

    void Hold(bool key)
    {
        if( holdCo != null )
        {
            StopCoroutine(holdCo);
        }
        holdCo = StartCoroutine(ExecHold( key ));
    }

    bool inAnim = false;

    IEnumerator ExecHold( bool key )
    {
     //   if (inAnim) yield break;

        inAnim = true;

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

        inAnim = false;
    }

    void UpdateHold()
    {
        foreach (FingerJoint f in fingers)
        {
            float angleTo = f.angle * currentHoldPercent;

            f.transform.localRotation = f.defaultRot * Quaternion.Euler(f.axis * angleTo);
        }

    }
}
