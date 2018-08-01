using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconTest : MonoBehaviour
{
    public int playerID = 1;
    public int speed;
    private Vector3 direction;

    [SerializeField]
    bool custom = false;

    [SerializeField]
    bool custom2 = false;

    void Start()
    {
    }

    void Update()
    {
        /*
        direction.Set(Input.GetAxis("JoyconH " + playerID), 0, Input.GetAxis("JoyconV " + playerID));
        if (direction != Vector3.zero) transform.rotation = Quaternion.LookRotation(direction);

        // Joy-Con(R)
        var h1 = Input.GetAxis("JoyconH 1");
        var v1 = Input.GetAxis("JoyconV 1");

        // Joy-Con(L)
        var h2 = Input.GetAxis("JoyconH 2");
        var v2 = Input.GetAxis("JoyconV 2");

        Debug.Log(string.Format(" JoyconH 1 = {0} JoyconV 1 = {1} JoyconH 2 = {2} JoyconV 2 = {3}", h1, v1, h2, v2));
        */
    }

    bool[] actionKeyReverse = new bool[4] { false, false, false, false};
    bool[][] buttonPressedPrev = new bool[4][]
    {
        new bool[15]{ false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
        new bool[15]{ false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
        new bool[15]{ false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
        new bool[15]{ false, false, false, false, false, false, false, false, false, false, false, false, false, false, false }
    };

    void OnGUI()
    {
        if( custom2 )
        {
            for (int playerIndex = 1; playerIndex <= 4; ++playerIndex)
            {
                // 問題：１つめのジョイパッドでぜんぶのジョイパッドが押された扱いになる

                var str = "JoyconDown" + playerIndex.ToString();
                if( Input.GetButton( str ) )
                {
                    Debug.Log(playerIndex.ToString() + " : おしている");
                }
                if (Input.GetButtonDown(str))
                {
                    Debug.Log(playerIndex.ToString() + " : おしはじめ");
                }
                if (Input.GetButtonUp(str))
                {
                    Debug.Log(playerIndex.ToString() + " : おしおわり");
                }
            }

        }
        if (custom)
        {
            for( int playerIndex=0; playerIndex <= 3; ++playerIndex )
            {
                bool changeReverseSettingOnce = false;

                for (int buttonIndex = 0; buttonIndex <= 15; ++buttonIndex)
                {
                    var keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + (playerIndex+1).ToString() + "Button" + buttonIndex.ToString());

                    if (buttonIndex <= 4)
                    {
                        if (GetKeyDown(keyCode, playerIndex, buttonIndex))
                        {
                            if (actionKeyReverse[playerIndex])
                            {
                                if (buttonIndex == 3)
                                {
                                    GUILayout.Label(playerIndex.ToString() + " : normal");
                                    Debug.Log(playerIndex.ToString() + " : normal");
                                }
                                else if (buttonIndex == 2)
                                {
                                    GUILayout.Label(playerIndex.ToString() + " : angey");
                                    Debug.Log(playerIndex.ToString() + " : angey");
                                }
                                else if (buttonIndex == 0)
                                {
                                    GUILayout.Label(playerIndex.ToString() + " : sad");
                                    Debug.Log(playerIndex.ToString() + " : sad");
                                }
                                else if (buttonIndex == 1)
                                {
                                    GUILayout.Label(playerIndex.ToString() + " : joy");
                                    Debug.Log(playerIndex.ToString() + " : joy");
                                }
                            }
                            else
                            {
                                if (buttonIndex == 0)
                                {
                                    GUILayout.Label(playerIndex.ToString() + " : normal");
                                    Debug.Log(playerIndex.ToString() + " : normal");
                                }
                                else if (buttonIndex == 1)
                                {
                                    GUILayout.Label(playerIndex.ToString() + " : angey");
                                    Debug.Log(playerIndex.ToString() + " : angey");
                                }
                                else if (buttonIndex == 3)
                                {
                                    GUILayout.Label(playerIndex.ToString() + " : sad");
                                    Debug.Log(playerIndex.ToString() + " : sad");
                                }
                                else if(buttonIndex == 2)
                                {
                                    GUILayout.Label(playerIndex.ToString() + " : joy");
                                    Debug.Log(playerIndex.ToString() + " : joy");
                                }
                            }
                        }
                    }

                    if (buttonIndex == 14)
                    {
                        if (Input.GetKey(keyCode))
                        {
                            GUILayout.Label(playerIndex.ToString() + " : ぱくぱく");
                        }
                    }
                    else if (buttonIndex == 10 || buttonIndex == 11)
                    {
                        if (GetKeyDown(keyCode, playerIndex, buttonIndex))
                        {
                            if (!changeReverseSettingOnce)
                            {
                                actionKeyReverse[playerIndex] = !actionKeyReverse[playerIndex];
                                GUILayout.Label((playerIndex + 1).ToString() + ":reverse = [" + actionKeyReverse[playerIndex].ToString() + "]");
                                Debug.Log((playerIndex + 1).ToString() + ":reverse = [" + actionKeyReverse[playerIndex].ToString() + "]");
                                changeReverseSettingOnce = true;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = (int)KeyCode.Joystick1Button0; i <= (int)KeyCode.Joystick2Button19; i++)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {
                    GUILayout.Label(((KeyCode)i).ToString() + " is pressed.");
                }
            }
        }
    }

    /// <summary>
    /// JoyconのKeyDownが効かない（GetKeyしか効かない）ので自作
    /// </summary>
    bool GetKeyDown( KeyCode keyCode, int playerIndex, int buttonIndex )
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
}