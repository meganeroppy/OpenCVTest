using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EggSystem;

/// <summary>
/// 平井ボーイなどの表情制御
/// </summary>
public class FacialController : MonoBehaviour
{
    /// <summary>
    /// 目のマテリアル
    /// </summary>
    [SerializeField]
    Material eyeMat;

    /// <summary>
    /// 口と眉毛のマテリアル
    /// </summary>
    [SerializeField]
    Material faceMat;

    /// <summary>
    /// 目のテクスチャ通常時
    /// </summary>
    [SerializeField]
    Texture[] eyeTexturesNormal;

    /// <summary>
    /// 目のテクスチャ驚き時
    /// </summary>
    [SerializeField]
    Texture[] eyeTexturesSurprized;

    Texture[] currentEyeTextures;

    /// <summary>
    /// 口テクスチャ開き通常時
    /// </summary>
    [SerializeField]
    Texture[] faceTexturesOpenNormal;

    /// <summary>
    /// 口テクスチャ閉じ通常時
    /// </summary>
    [SerializeField]
    Texture faceTexturesCloseNormal;

    Texture[] currentFaceTexturesOpen;

    Texture currentFaceTexturesClose;

    [SerializeField]
    float blinkIntervalMin = 2f;
    [SerializeField]
    float blinkIntervalMax = 5f;

    float blinkInterval;

    float blinkTimer = 0;

    Coroutine blinkCoroutine = null;

    bool talking = false;

    bool mouthClose = true;

    float mouthChangeTimer = 0;

    [SerializeField]
    float mouthChangeInterval = 0.25f;

    [SerializeField]
    bool autoTalk = false;

    private void Awake()
    {
        currentEyeTextures = eyeTexturesNormal;
        currentFaceTexturesOpen = faceTexturesOpenNormal;
        currentFaceTexturesClose = faceTexturesCloseNormal;
    }

    private void Update()
    {
        UpdateBlinkTimer();
        UpdateMouthTimer();
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
        int count = 0;

        while (count < repeat)
        {
            // 目をつぶる
            Blink(2);

            yield return new WaitForSeconds(0.25f);

            // 目を少し開ける
            Blink(1);

            yield return new WaitForSeconds(0.1f);

            // 目を開ける
            Blink(0);

            yield return new WaitForSeconds(0.25f);

            count++;
        }

        blinkCoroutine = null;
    }

    /// <summary>
    /// まばたき
    /// </summary>
    public void Blink(int key)
    {
        if (currentEyeTextures.Length < 2)
        {
            Debug.LogWarning(" 現在の目はまばたきなし ");
            return;
        }

        // 目をつぶる
        eyeMat.mainTexture = currentEyeTextures[key % currentEyeTextures.Length];
    }

    /// <summary>
    /// 口パク更新
    /// </summary>
    void UpdateMouthTimer()
    {
        if (!talking && !autoTalk)
        {
            mouthChangeTimer += Time.deltaTime;
            if (mouthChangeTimer < mouthChangeInterval) return;

            UpdateMouth(false, 0);

            // 次回口パク状態になったら即口が開いている状態になるようにする
            mouthChangeTimer = mouthChangeInterval;
            mouthClose = true;
        }
        else
        {
            mouthChangeTimer += Time.deltaTime;
            if (mouthChangeTimer < mouthChangeInterval) return;

            mouthChangeTimer = 0;

            if (mouthClose)
            {
                UpdateMouth(true, 0);
            }
            else
            {
                UpdateMouth(false, 0);
            }

            mouthClose = !mouthClose;

            // 次のフレーム開始時に再度trueになっていなければ口パク終了
            talking = false;
        }
    }

    /// <summary>
    /// 口パク更新
    /// </summary>
    public void UpdateMouth(bool open, int key)
    {
        if (open)
        {
            var newFaceTex = currentFaceTexturesOpen.Length == 1 ? currentFaceTexturesOpen[0] : currentFaceTexturesOpen[Random.Range(0, currentFaceTexturesOpen.Length)];
            faceMat.mainTexture = newFaceTex;
        }
        else
        {
            faceMat.mainTexture = currentFaceTexturesClose;
        }
    }

    public void SetFacial(Facial newFaicial)
    {
        // 目のタイプを更新
        switch (newFaicial)
        {
            case Facial.Normal:
                currentEyeTextures = eyeTexturesNormal;
                currentFaceTexturesOpen = faceTexturesOpenNormal;
                currentFaceTexturesClose = faceTexturesCloseNormal;
                break;
            case Facial.Angry:
            //    currentEyeMaterials = currentContentParts.EyeMaterials_surprized;
            //    currentMouthOpenMaterials = currentContentParts.MouthOpenMaterials_surprized;
            //    currentMouthCloseMaterial = currentContentParts.MouthCloseMaterial_surprited;
                break;
            case Facial.Sad:
            //    currentEyeMaterials = currentContentParts.EyeMaterial_sad;
            //    currentMouthOpenMaterials = currentContentParts.MouthOpenMaterials_negative;
            //    currentMouthCloseMaterial = currentContentParts.MouthCloseMaterial_negative;
                break;
            case Facial.Smile:
            //    currentEyeMaterials = currentContentParts.EyeMaterial_smile;
            //    currentMouthOpenMaterials = currentContentParts.MouthOpenMaterials;
            //    currentMouthCloseMaterial = currentContentParts.MouthCloseMaterial_smile;
                break;

            default:
                break;
        }

        // 更新された表情セットでデフォルトの表情
        eyeMat.mainTexture = currentEyeTextures[0];
        faceMat.mainTexture = currentFaceTexturesClose;
    }

}
