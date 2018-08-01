using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class VRMFacialController : MonoBehaviour
{
    [SerializeField]
    VRMBlendShapeProxy vrmBlendShape;
    
    [SerializeField]
    float blinkIntervalMin = 2f;
    [SerializeField]
    float blinkIntervalMax = 5f;

    float blinkInterval;

    float blinkTimer = 0;

    Coroutine blinkCoroutine = null;

	[SerializeField]
	float blinkSpeed = 10f;

    BlendShapePreset currentBlendShape;

    [SerializeField]
    SteamVR_TrackedObject[] controller;

    private void Awake()
    {
        currentBlendShape = BlendShapePreset.Neutral;
        vrmBlendShape.SetValue(currentBlendShape, 1, true);
    }

    private void Update()
    {
        UpdateBlinkTimer();
        UpdateInput();
    }

    void UpdateBlinkTimer()
    {
        // 笑顔の時はまばたきなし
        if (currentBlendShape == BlendShapePreset.Smile) return;

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
        int count = 0;
        float progress = 0;
        float value = 0;
		float blink = 0;

//        float blinkOpenVal = 0.67f;
        float blinkOpenVal = 1f;

        while (count < repeat)
        {
            progress = 0;
            while( progress < 1f )
            {
                progress += Time.deltaTime * blinkSpeed;
				value = progress;

				vrmBlendShape.SetValue(currentBlendShape, value, false);

				blink = Mathf.Lerp(0.25f, 1.0f, value);

				vrmBlendShape.SetValue(VRM.BlendShapePreset.Blink, blink, false);

				vrmBlendShape.Apply();

                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            progress = 0;
            while (progress < blinkOpenVal)
            {
                progress += Time.deltaTime * (blinkSpeed / 2);
                value = 1f - progress;

				vrmBlendShape.SetValue(currentBlendShape, value, false);

				blink = Mathf.Lerp(0.25f, 1.0f, value);

				vrmBlendShape.SetValue(VRM.BlendShapePreset.Blink, blink, false);

				vrmBlendShape.Apply();

                yield return null;
            }

            yield return new WaitForSeconds(0.25f);

            count++;
        }

        // まばたきあとにニュートラルに戻す TODO:瞬きするまえの表情にもどす
        vrmBlendShape.SetValue(currentBlendShape, 1);

        blinkCoroutine = null;
    }

    [SerializeField]
    float touchPadThreshold = 0.7f;

    void UpdateInput()
    {
        if( Input.GetKeyDown(KeyCode.Q))
        {
            SetFacialJoy();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetFacialAngry();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetFacialSad();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetFacialHappy();
        }
		if (Input.GetKeyDown(KeyCode.T))
		{
			SetFacialNeutral();
		}

        // Vive入力関連
        {
			if( controller == null ) return;

            for (int i = 0; i < controller.Length; ++i)
            {
                var device = SteamVR_Controller.Input((int)controller[i].index);

                if (device == null) return;

                // タッチパッドのタッチ位置を取得
                var position = device.GetAxis();

                if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
                {
                    if (position.y < -touchPadThreshold && Mathf.Abs(position.x) <= touchPadThreshold)
                    {
                        SetFacial(BlendShapePreset.Smile);
                    }
                    if (position.x > touchPadThreshold && Mathf.Abs(position.y) <= touchPadThreshold)
                    {
                        SetFacial(BlendShapePreset.Angry);
                    }
                    if (position.y > touchPadThreshold && Mathf.Abs(position.x) <= touchPadThreshold)
                    {
                        SetFacial(BlendShapePreset.Sorrow);
                    }
                    if (position.x < -touchPadThreshold && Mathf.Abs(position.y) <= touchPadThreshold)
                    {
                        SetFacial(BlendShapePreset.Fun);
                    }
                    if (Mathf.Abs(position.x) <= touchPadThreshold && Mathf.Abs(position.y) <= touchPadThreshold)
                    {
                        SetFacial(BlendShapePreset.Neutral);
                    }
                }
            }
        }
    }

    [ContextMenu("Neutral")]
    public void SetFacialNeutral()
    {
        SetFacial(BlendShapePreset.Neutral);
    }

    [ContextMenu("Joy")]
    public void SetFacialJoy()
    {
//        SetFacial(BlendShapePreset.Joy);
        SetFacial(BlendShapePreset.Smile);
    }
    [ContextMenu("Angry")]
    public void SetFacialAngry()
    {
        SetFacial(BlendShapePreset.Angry);
    }
    [ContextMenu("Sad")]
    public void SetFacialSad()
    {
        SetFacial(BlendShapePreset.Sorrow);
    }
    [ContextMenu("Happy")]
    public void SetFacialHappy()
    {
        SetFacial(BlendShapePreset.Fun);
    }

    public void SetFacial( BlendShapePreset newPreset )
    {
        var prev = currentBlendShape;
        currentBlendShape = newPreset;

		vrmBlendShape.SetValue(currentBlendShape, 1, false);

		if( prev != currentBlendShape )
		{
			vrmBlendShape.SetValue(prev, 0, false);
		}
    
		if( currentBlendShape == BlendShapePreset.Smile)
        {
            if( blinkCoroutine != null)
			{
                StopCoroutine(blinkCoroutine);
				vrmBlendShape.SetValue(BlendShapePreset.Blink, 0, false);
			}
        }

		vrmBlendShape.Apply();

//		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod() +  string.Format("prev = {0} new = {1}", prev.ToString(), newPreset.ToString() ));

    }
}
