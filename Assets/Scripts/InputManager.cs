using System.Collections;
using System.Collections.Generic;
using EggSystem;
using UnityEngine;
using GamepadInput;

/// <summary>
/// 入力管理
/// </summary>
public class InputManager : MonoBehaviour {

    [SerializeField]
    SteamVR_TrackedObject controller;

    /// <summary>
    /// 役割が入れ替わってしまったときに入れ替える対象
    /// </summary>
    [SerializeField]
    SteamVR_TrackedObject otherController;


    [SerializeField]
    SteamVR_TrackedObject[] trackers;

    /// <summary>
    /// トラッカーカメラのビジュアル
    /// </summary>
    [SerializeField]
    GameObject cameraModel;

    [SerializeField]
    bool logViveTouchInput = false;

    KeyCode[] normalFaceKeyboardAssin = new KeyCode[3] { KeyCode.Q, KeyCode.A, KeyCode.Z };
    KeyCode[] angryFaceKeyboardAssin = new KeyCode[3] { KeyCode.W, KeyCode.S, KeyCode.X };
    KeyCode[] sadFaceKeyboardAssin = new KeyCode[3] { KeyCode.E, KeyCode.D, KeyCode.C };
    KeyCode[] smileFaceKeyboardAssin = new KeyCode[3] { KeyCode.R, KeyCode.F, KeyCode.V };
    KeyCode[] talkKeyboardAssin = new KeyCode[3] { KeyCode.T, KeyCode.G, KeyCode.B };
    KeyCode[] changePetternKeyboardAssin = new KeyCode[3] { KeyCode.Y, KeyCode.H, KeyCode.N };
    KeyCode[] hatchKeyboardAssin = new KeyCode[3] { KeyCode.U, KeyCode.J, KeyCode.M };
    const int MaxPlayer = 4;

    void Update()
    {
        if (EggGameManager.instance == null) return;

        var gm = EggGameManager.instance;

        bool withShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // たまご柄関連
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gm.SetEggPettern(false);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                gm.SetEggPettern(true);
            }
        }

        // カメラ関連
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                gm.camName.enabled = !gm.camName.enabled;
            }

            //カメラモデル表示切替
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!cameraModel) return;
                cameraModel.SetActive(!cameraModel.activeInHierarchy);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                gm.SetActiveCamera(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                gm.SetActiveCamera(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                gm.SetActiveCamera(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                gm.SetActiveCamera(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                gm.SetActiveCamera(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                gm.SetActiveCamera(5);
            }
        }

        // エフェクト関連 / 表情関連
        {
            for (int playerIndex = 0; playerIndex < MaxPlayer; ++playerIndex)
            {
                if (normalFaceKeyboardAssin.Length > playerIndex)
                {
                    // 通常顔 Q,A,Z
                    if (Input.GetKeyDown(normalFaceKeyboardAssin[playerIndex]))
                    {
                        gm.SetEggFacial(Facial.Normal, playerIndex);
                    }

                    // 怒り（ショック）顔 W,S,X
                    if (Input.GetKeyDown(angryFaceKeyboardAssin[playerIndex]))
                    {
                        gm.SetEggFacial(Facial.Angry, playerIndex);
                    }

                    // 悲しみ顔 E,D,C
                    if (Input.GetKeyDown(sadFaceKeyboardAssin[playerIndex]))
                    {
                        gm.SetEggFacial(Facial.Sad, playerIndex);
                    }

                    // 笑顔 R,F,V
                    if (Input.GetKeyDown(smileFaceKeyboardAssin[playerIndex]))
                    {
                        gm.SetEggFacial(Facial.Smile, playerIndex);
                    }

                    // 口パク T,G,B
                    if (Input.GetKey(talkKeyboardAssin[playerIndex]))
                    {
                        gm.SetEggTalk(playerIndex);
                    }

                    // 柄変更 Y,H,N
                    if (Input.GetKeyDown(changePetternKeyboardAssin[playerIndex]))
                    {
                        gm.SetEggPettern(!withShift, playerIndex);
                    }

                    // 孵化 U,J,M
                    if (Input.GetKeyDown(hatchKeyboardAssin[playerIndex]))
                    {
                        gm.SetEggHatch(!withShift, playerIndex);
                    }
                }

                // ジョイパッド入力
                if (GamePad.GetButtonDown(GamePad.Button.A, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggFacial(Facial.Normal, playerIndex);
                }
                else if (GamePad.GetButtonDown(GamePad.Button.B, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggFacial(Facial.Angry, playerIndex);
                }
                else if (GamePad.GetButtonDown(GamePad.Button.Y, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggFacial(Facial.Sad, playerIndex);
                }
                else if (GamePad.GetButtonDown(GamePad.Button.X, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggFacial(Facial.Smile, playerIndex);
                }

                // 口パク右肩ボタン（トリガーではない）
                if (GamePad.GetButton(GamePad.Button.RightShoulder, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggTalk(playerIndex);
                }

                // 柄変更 スタートボタン
                if (GamePad.GetButtonDown(GamePad.Button.Start, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggPettern(true, playerIndex);
                }
                // 柄変更逆準 バックボタン
                else if (GamePad.GetButtonDown(GamePad.Button.Back, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggPettern(false, playerIndex);
                }

                // 孵化 右スティック押し込み
                if (GamePad.GetButtonDown(GamePad.Button.RightStick, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggHatch(true, playerIndex);
                }
                // もとにもどる 左スティック押し込み
                else if (GamePad.GetButtonDown(GamePad.Button.LeftStick, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggHatch(false, playerIndex);
                }

                if (GamePad.GetButtonDown(GamePad.Button.A, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggFacial(Facial.Normal, playerIndex);
                }
                else if (GamePad.GetButtonDown(GamePad.Button.B, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggFacial(Facial.Angry, playerIndex);
                }
                else if (GamePad.GetButtonDown(GamePad.Button.Y, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggFacial(Facial.Sad, playerIndex);
                }
                else if (GamePad.GetButtonDown(GamePad.Button.X, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggFacial(Facial.Smile, playerIndex);
                }

                // 口パク右肩ボタン（トリガーではない）
                if (GamePad.GetButton(GamePad.Button.RightShoulder, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggTalk(playerIndex);
                }

                // 柄変更 スタートボタン
                if (GamePad.GetButtonDown(GamePad.Button.Start, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggPettern(true, playerIndex);
                }
                // 柄変更逆準 バックボタン
                else if (GamePad.GetButtonDown(GamePad.Button.Back, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggPettern(false, playerIndex);
                }

                // 孵化 右スティック押し込み
                if (GamePad.GetButtonDown(GamePad.Button.RightStick, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggHatch(true, playerIndex);
                }
                // もとにもどる 左スティック押し込み
                else if (GamePad.GetButtonDown(GamePad.Button.LeftStick, (GamePad.Index)(playerIndex + 1)))
                {
                    gm.SetEggHatch(false, playerIndex);
                }

                // Joy-Con入力
                bool changeReverseSettingOnce = false;

                for (int buttonIndex = 0; buttonIndex <= 15; ++buttonIndex)
                {
                    var keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + (playerIndex + 1).ToString() + "Button" + buttonIndex.ToString());

                    if (buttonIndex <= 4)
                    {
                        if (JoyconGetKeyDown(keyCode, playerIndex, buttonIndex))
                        {
                            if (actionKeyReverse[playerIndex])
                            {
                                if (buttonIndex == 3)
                                {
                                    Debug.Log(playerIndex.ToString() + " : normal");
                                    gm.SetEggFacial(Facial.Normal, playerIndex);
                                }
                                else if (buttonIndex == 2)
                                {
                                    Debug.Log(playerIndex.ToString() + " : angey");
                                    gm.SetEggFacial(Facial.Angry, playerIndex);
                                }
                                else if (buttonIndex == 0)
                                {
                                    Debug.Log(playerIndex.ToString() + " : sad");
                                    gm.SetEggFacial(Facial.Sad, playerIndex);
                                }
                                else if (buttonIndex == 1)
                                {
                                    Debug.Log(playerIndex.ToString() + " : joy");
                                    gm.SetEggFacial(Facial.Smile, playerIndex);
                                }
                            }
                            else
                            {
                                if (buttonIndex == 0)
                                {
                                    Debug.Log(playerIndex.ToString() + " : normal");
                                    gm.SetEggFacial(Facial.Normal, playerIndex);
                                }
                                else if (buttonIndex == 1)
                                {
                                    Debug.Log(playerIndex.ToString() + " : angey");
                                    gm.SetEggFacial(Facial.Angry, playerIndex);
                                }
                                else if (buttonIndex == 3)
                                {
                                    Debug.Log(playerIndex.ToString() + " : sad");
                                    gm.SetEggFacial(Facial.Sad, playerIndex);
                                }
                                else if (buttonIndex == 2)
                                {
                                    Debug.Log(playerIndex.ToString() + " : joy");
                                    gm.SetEggFacial(Facial.Smile, playerIndex);
                                }
                            }
                        }
                    }

                    if (buttonIndex == 14)
                    {
                        if (Input.GetKey(keyCode))
                        {
                            gm.SetEggTalk(playerIndex);
                            Debug.Log(playerIndex.ToString() + " : ぱくぱく");
                        }
                    }
                    else if (buttonIndex == 10 || buttonIndex == 11)
                    {
                        if (JoyconGetKeyDown(keyCode, playerIndex, buttonIndex))
                        {
                            if (!changeReverseSettingOnce)
                            {
                                actionKeyReverse[playerIndex] = !actionKeyReverse[playerIndex];
                                Debug.Log((playerIndex + 1).ToString() + ":reverse = [" + actionKeyReverse[playerIndex].ToString() + "]");
                                changeReverseSettingOnce = true;
                            }
                        }
                    }
                }
            }
        }
    

        // たまごアサイン関連
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                gm.AssignEggToTrackers();
            }
        }
        
        // シーン変更関連
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
               StartCoroutine( gm.GotoNextScene() );
            }
        }
        
        // Vive入力関連
        {
            if (controller == null) return;
            var device = SteamVR_Controller.Input((int)controller.index);

            if (device == null) return;

            // タッチパッドのタッチ位置を取得
            var position = device.GetAxis();
            if (logViveTouchInput)
                Debug.Log("x: " + position.x + " y: " + position.y);


            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                if( logViveTouchInput )
                Debug.Log("トリガーを浅く引いた");
            }
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                if (logViveTouchInput)
                    Debug.Log("トリガーを深く引いた");

                gm.SetActiveCamera(gm.TempSelectIndex);
            }
            if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                if (logViveTouchInput)
                    Debug.Log("トリガーを離した");
            }
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                if (logViveTouchInput)
                    Debug.Log("タッチパッドをクリックした");

                if( position.x > 0)
                {
                    gm.SetCameraTempIndex(true);
                }
                else
                {
                    gm.SetCameraTempIndex(false);
                }

            }
            if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
            {
                if (logViveTouchInput)
                    Debug.Log("タッチパッドをクリックしている");
            }
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                if (logViveTouchInput)
                    Debug.Log("タッチパッドをクリックして離した");
            }
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                if (logViveTouchInput)
                    Debug.Log("タッチパッドに触った");
            }
            if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                if (logViveTouchInput)
                    Debug.Log("タッチパッドを離した");
            }
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                if (logViveTouchInput)
                    Debug.Log("メニューボタンをクリックした");
            }
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                if (logViveTouchInput)
                    Debug.Log("グリップボタンをクリックした");

                StartCoroutine(gm.GotoNextScene());
            }

            if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
            {
                //Debug.Log("トリガーを浅く引いている");
            }
            if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                //Debug.Log("トリガーを深く引いている");
                switchTimer += Time.deltaTime;
            }
            if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
            {
                //Debug.Log("タッチパッドに触っている");
            }

            // 役割が入れ替わっているときの処理
            {
                device = SteamVR_Controller.Input((int)otherController.index);

                if (device == null) return;
                if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
                {
                    //Debug.Log("トリガーを深く引いている");
                    switchTimer += Time.deltaTime;
                }

                if (switchTimer > 5f)
                {
                    switchTimer = 0;
                    SwtichControllerRole();
                }
            }

            // トラッカーが切れてしまった時用の復帰処理テスト
            {
                if( Input.GetKeyDown( KeyCode.K ))
                {
                    forceReactivateTrackedObjects();
                }
            }
        }
    }

    float switchTimer = 0;

    /// <summary>
    /// 役割が意図せず入れ替わってしまった時用
    /// </summary>
    void SwtichControllerRole()
    {
        var temp = controller.index;
        controller.index = otherController.index;
        otherController.index = temp;
    }

    void forceReactivateTrackedObjects()
    {
        if (controller != null)
            controller.gameObject.SetActive(true);

        if (otherController != null)
            otherController.gameObject.SetActive(true);

        if (trackers != null)
        {
            foreach (SteamVR_TrackedObject o in trackers)
            {
                if (o != null)
                    o.gameObject.SetActive(true);
            }
        }
    }

    #region joy-con 

    bool[] actionKeyReverse = new bool[4] { false, false, false, false };
    bool[][] buttonPressedPrev = new bool[4][]
    {
        new bool[15]{ false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
        new bool[15]{ false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
        new bool[15]{ false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
        new bool[15]{ false, false, false, false, false, false, false, false, false, false, false, false, false, false, false }
    };

    /// <summary>
    /// JoyconのKeyDownが効かない（GetKeyしか効かない）ので自作
    /// </summary>
    bool JoyconGetKeyDown(KeyCode keyCode, int playerIndex, int buttonIndex)
    {
        bool ret;
        // このフレームで押されているか？
        var pressed = Input.GetKey(keyCode);

        if (!pressed)
        {
            ret = false;
            buttonPressedPrev[playerIndex][buttonIndex] = false;
        }
        else
        {
            // 初めて押されたか？
            var pressDownAtThisFrame = !buttonPressedPrev[playerIndex][buttonIndex];
            if (pressDownAtThisFrame)
            {
                ret = true;

            }
            else
            {
                ret = false;
            }

            buttonPressedPrev[playerIndex][buttonIndex] = true;
        }

        return ret;
    }

    #endregion joy-con
}
