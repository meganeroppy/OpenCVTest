using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// くま制御
/// </summary>
public class KumaController : MonoBehaviour {

    #region Transition

    enum Mode
    {
        MotionCapture,
        Sit,
    }

    Mode currentMode = Mode.Sit;

    [SerializeField]
    Image frontImage;
    
    [SerializeField]
    GameObject chairObj;

    [SerializeField]
    GameObject plate;

    [SerializeField]
    float fadeDur = 2f;

    bool inTransition = false;

    #endregion Transition

    #region Blink

    [SerializeField]
    SkinnedMeshRenderer kumaMesh;

    [SerializeField]
    float eyeCloseDur = 0.1f;
    [SerializeField]
    float eyeOpenDur = 0.2f;

    [SerializeField]
    float blinkIntervalMin = 2f;
    [SerializeField]
    float blinkIntervalMax = 5f;

    float blinkInterval;

    float blinkTimer = 0;

    Coroutine blinkCoroutine = null;

    #endregion Blink

    #region Anim

    Animator anim;

    [SerializeField]
    int sitAnimIndex = 1;

    const int MaxAnimIndex = 7;

    [SerializeField]
    FingerAnim[] hands;

    #endregion Anim

    #region Sit
    [SerializeField]
    Vector3 sitPos = Vector3.up * 0.1f;
    [SerializeField]
    Vector3 SitRot = new Vector3( -30f, 30, 0 );

    #endregion Sit

    #region finalIK

    [SerializeField]
    Transform cameraRig;

    RootMotion.FinalIK.VRIK vrik;

    [SerializeField]
    SteamVR_TrackedObject[] controllers;

    #endregion finalIK

    [SerializeField]
    Wine wine;

    private void Start()
    {
        anim = GetComponent<Animator>();
        vrik = GetComponent<RootMotion.FinalIK.VRIK>();

        SetComponentEnable();
    }

    private void Update()
    {
        UpdateBlinkTimer();
        UpdateInput();
    }

    void UpdateBlinkTimer()
    {
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            // 次のまばたきまでの時間をセット
            blinkTimer = 0;
            blinkInterval = Random.Range(blinkIntervalMin, blinkIntervalMax);

            // 時々2連続
            int repeat = Random.Range(0, 5) == 0 ? 2 : 1;

            blinkCoroutine = StartCoroutine(BlinkCoroutine(repeat));
        }
    }

    /// <summary>
    /// まばたき
    /// </summary>
    IEnumerator BlinkCoroutine(int repeat = 1)
    {
        if (kumaMesh == null) yield break;

        int count = 0;
        float progress;
        float valuePerFrame;

        while (count < repeat)
        {
            // 目をつぶる
            {
                progress = 0;
                valuePerFrame = 1f / eyeCloseDur;

                while (progress < 1)
                {
                    progress += valuePerFrame * Time.deltaTime;

                    kumaMesh.SetBlendShapeWeight(0, progress * 100);

                    yield return null;
                }
            }

            // 若干待機
            yield return new WaitForSeconds(0.1f);

            // 目を開ける
            {
                progress = 0;
                valuePerFrame = 1f / eyeOpenDur;

                while (progress < 1)
                {
                    progress += valuePerFrame * Time.deltaTime;

                    kumaMesh.SetBlendShapeWeight(0, ( 1f - progress ) * 100);

                    yield return null;
                }
            }

            count++;
        }

        blinkCoroutine = null;
    }

    void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SwitchMode();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if( plate )
            {
                plate.SetActive(!plate.activeInHierarchy);
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if( wine != null )
            {
                wine.Set(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (wine != null)
            {
                wine.Set(false);
            }
        }

        for (int i = 0; i < controllers.Length; ++i)
        {
            var device = SteamVR_Controller.Input((int)controllers[i].index);
            if (device == null) continue;

            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                SwitchMode();
            }

            // カメラリグの高さを変更
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                var position = device.GetAxis();

                if (position.y > 0)
                {
                    cameraRig.transform.position += Vector3.up * 0.025f;
                }
                else
                {
                    cameraRig.transform.position -= Vector3.up * 0.025f;
                }
            }

        }
    }
    void SwitchMode()
    {
        StartCoroutine(Transition());
    }

    void SetComponentEnable()
    {
        anim.enabled = currentMode == Mode.Sit;
        chairObj.SetActive(currentMode == Mode.Sit);

        vrik.enabled = currentMode == Mode.MotionCapture;

        transform.localPosition = currentMode == Mode.Sit ? sitPos : Vector3.zero;
        transform.localRotation = Quaternion.Euler( currentMode == Mode.Sit ? SitRot : Vector3.zero );

        if (currentMode == Mode.Sit)
        {
            anim.SetInteger("Sit", sitAnimIndex);
        }
        else
        {
            foreach( FingerAnim f in hands )
            {
                f.SetHandDefault();
            }

            if( plate )
            {
                plate.SetActive(false);
            }
        }
    }

    IEnumerator Transition()
    {
        if (inTransition) yield break;

        currentMode = currentMode == Mode.Sit ? Mode.MotionCapture : Mode.Sit;

        inTransition = true;

        float progress;
        float valuePerFrame = 1f / fadeDur;
        Color color;

        // フェードアウト
        {
            progress = 0;
            while (progress < 1f)
            {
                progress += valuePerFrame * Time.deltaTime;
                color = frontImage.color;
                color.a = progress;
                frontImage.color = color;

                yield return null;
            }
        }

        yield return new WaitForSeconds(.25f);

        SetComponentEnable();

        yield return new WaitForSeconds(.25f);

        // フェードアウト
        {
            progress = 0;
            while (progress < 1f)
            {
                progress += valuePerFrame * Time.deltaTime;
                color = frontImage.color;
                color.a = 1f - progress;
                frontImage.color = color;

                yield return null;
            }
        }

        inTransition = false;
    }

}
