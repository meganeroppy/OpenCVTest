using UnityEngine;
using System.Collections;

public class PlayMicSound : MonoBehaviour
{
    void Start()
    {
        AudioSource aud = GetComponent<AudioSource>();
        // マイク名、ループするかどうか、AudioClipの秒数、サンプリングレート を指定する
        aud.clip = Microphone.Start(null, true, 10, 44100);
        aud.Play();
    }
}