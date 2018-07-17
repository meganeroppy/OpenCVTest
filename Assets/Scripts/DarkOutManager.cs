using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 暗転マネージャ
/// </summary>
public class DarkOutManager : MonoBehaviour
{
    public static DarkOutManager instance;

    [SerializeField]
    UnityEngine.UI.Image image;

    void Awake()
    {
        instance = this;
    }

    public IEnumerator Fade(bool toOut, float dur = 1f)
    {
        float progress = 0;
        Color newColor = Color.black;
        float valPerFrame = 1f / dur; 


        while( progress < 1 )
        {
            progress += Time.deltaTime * valPerFrame;
            newColor.a = toOut ? progress : (1f - progress);

            image.color = newColor;

            yield return null;
        }

//        Debug.Log("フェード完了");

    }
}
