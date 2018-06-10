using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラの映像を映す
/// 顔認識とかはしない
/// </summary>
public class WebCamController : MonoBehaviour
{
	int width = 1920;
	int height = 1080;
	int fps = 30;
	WebCamTexture webcamTexture;

    [SerializeField]
    int cameraIndex = 0;
	void Start () {
		WebCamDevice[] devices = WebCamTexture.devices;
        cameraIndex = cameraIndex % devices.Length;

		webcamTexture = new WebCamTexture(devices[cameraIndex].name, this.width, this.height, this.fps);
		GetComponent<Renderer> ().material.mainTexture = webcamTexture;
		webcamTexture.Play();
	}
}