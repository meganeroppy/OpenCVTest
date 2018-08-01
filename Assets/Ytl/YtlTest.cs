using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YouTubeLive {
  [RequireComponent(typeof(YtlController))]
  public class YtlTest : MonoBehaviour
    {
        [SerializeField]
        AudioSource source;

    void Start () {
      var ctrl = GetComponent<YtlController> ();
      ctrl.OnMessage += msg => {
        Debug.Log(msg.name + ": " + msg.text);
          source.Play();
      };
    }
  }
}
