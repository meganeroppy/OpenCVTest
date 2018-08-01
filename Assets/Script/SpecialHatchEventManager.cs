using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using CrazyMinnow.SALSA;

/// <summary>
/// 最初に登場したたまごにスペシャル孵化イベントを仕込む
/// </summary>
public class SpecialHatchEventManager : MonoBehaviour
{
    /// <summary>
    /// このゲームオブジェクトを中身とする
    /// </summary>
    [SerializeField]
    RootMotion.FinalIK.VRIK content = null;

    EggController targetEgg = null;

    AudioSource audioSource = null;

    [SerializeField]
    AudioMixerGroup mixerGroup;

    Salsa3D salsa;

    MicInput micInput;

    [SerializeField]
    float threshold = 0;

    [SerializeField]
    bool log = false;

    private void Start()
    {
        StartCoroutine(FindTargetEgg());
        content.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (targetEgg == null) return;

        //        Debug.Log(micInput.GetLoudness());
 //       var val = salsa.average;
        var value = MicInput.MicLoudness;

        if (log)
        {
            Debug.Log(value);
        }

        targetEgg.talking = value > threshold;
    }

    IEnumerator FindTargetEgg()
    {
        var gm = EggGameManager.instance;

        while (gm.EggList == null) yield return null;
        while (gm.EggList.Count < 1) yield return null;

        targetEgg = gm.EggList[0].GetComponent<EggController>();

        if (targetEgg == null) yield break;

        targetEgg.AssignToTracker(content.solver.spine.headTarget.gameObject);

        targetEgg.trackerOffetPos = Vector3.zero;
        targetEgg.trackerOffetRot = Vector3.zero;

        var objects = targetEgg.GetComponentInChildren<Transform>();

        foreach( Transform t in objects )
        {
            t.gameObject.layer = LayerMask.NameToLayer("Avatar");
        }

        targetEgg.enableSpecialContent = true;
        
        targetEgg.specialHatchEvent = () => { content.gameObject.SetActive(true); };
        targetEgg.specialUnhatchEvent = () => { content.gameObject.SetActive(false); };
        
        gameObject.AddComponent<CM_MicInput>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixerGroup;

    //    salsa = gameObject.AddComponent<Salsa3D>();
    //    salsa.audioSrc = audioSource;
        
        micInput = gameObject.AddComponent<MicInput>();
    }
}
