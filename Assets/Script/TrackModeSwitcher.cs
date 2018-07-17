using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;

/// <summary>
/// iKinemaトラッキングとMMDモーションを切り替える
/// </summary>
public class TrackModeSwitcher : MonoBehaviour {

    public enum MotionType
    {
        FinalIK,
        MMMD,
        Count
    }

    [SerializeField]
    Image dark;

    [SerializeField]
    Image logo;

    [SerializeField]
    MotionType motionType;

    [SerializeField]
    VRIK vrik;

    [SerializeField]
    Animator animator;

    [SerializeField]
    float fadeSpeed = 3f;

    bool inProgress = false;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(ExecSwtich(true) );
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.S) )
        {
            StartCoroutine(ExecSwtich());
        }
	}

    IEnumerator ExecSwtich( bool notChange=false )
    {
        if (inProgress) yield break;

        inProgress = true;

        // 暗転
        yield return Fade(true);

        if( !notChange )
            motionType = (MotionType)(((int)motionType + 1) % (int)MotionType.Count);

        vrik.enabled = motionType == MotionType.FinalIK;
        animator.enabled = motionType == MotionType.MMMD;

        yield return new WaitForSeconds(1f);

        yield return Fade(false);

        inProgress = false;
    }

    IEnumerator Fade(bool toIn)
    {
        float progress = 0;

        float newAlpha;
        Color darkColor = Color.black;
        Color logoColor = Color.white;
        var valuePerFrame = 1f / fadeSpeed;
        while( progress < 1f)
        {
            progress += valuePerFrame * Time.deltaTime;

            newAlpha = toIn ? progress : 1f - progress;

            darkColor.a = newAlpha;
            dark.color = darkColor;
            logoColor.a = newAlpha;
            logo.color = logoColor;

            yield return null;
        }
    }
}
