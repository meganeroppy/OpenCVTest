using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMirroring : MonoBehaviour {

    [SerializeField]
    RawImage rawImage = null;

    [SerializeField]
    Camera[] cameras = null;

    int currentIndex = 0;

	// Update is called once per frame
	void Update () {

        for (int i = 0; i < cameras.Length; ++i)
        {
            rawImage.texture = cameras[EggGameManager.instance.CurrentCameraIndex].targetTexture;
        }
    }
}
