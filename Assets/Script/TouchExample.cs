using UnityEngine;
using Valve.VR;

public class TouchExample : MonoBehaviour
{
    void Update()
    {
        var trackedObject = GetComponent<SteamVR_TrackedObject>();
        var device = SteamVR_Controller.Input((int)trackedObject.index);

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("人差し指トリガーに触った");
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("人差し指トリガーを離した");
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("人差し指トリガーを引いた");
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("人差し指トリガーを引いて離した");
        }

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("スティックをクリックした");
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("スティックをクリックして離した");
        }
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("スティックに触った");
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("スティックを離した");
        }

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("中指トリガーを引いた");
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("中指トリガーを引いて離した");
        }

        // A/Xボタンは正しく認識されない？
        //if (device.GetTouchDown((int)EVRButtonId.k_EButton_A))
        //{
        //    Debug.Log("A/Xボタンに触った");
        //}
        //if (device.GetPressDown((int)EVRButtonId.k_EButton_A))
        //{
        //    Debug.Log("A/Xボタンをクリックした");
        //}

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Debug.Log("B/Yボタンに触った");
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Debug.Log("B/Yボタンをクリックした");
        }

        //if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        //{
        //    //Debug.Log("人差し指トリガーを引いている");
        //}
        //if (device.GetPress(SteamVR_Controller.ButtonMask.Grip))
        //{
        //    Debug.Log("中指トリガーを引いている");
        //}
    }
}