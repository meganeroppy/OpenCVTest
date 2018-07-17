using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerAnim : MonoBehaviour
{
    enum FingerEnum
    {
        Thumb,
        Index,
        Middle,
        Ring,
        Pinky,
        Count,
    }

    enum HandEnum
    {
        Rock,
        Scisser,
        Paper,
        Point,
        Count,
    }

    HandEnum currentGoalHand = HandEnum.Rock;
    HandEnum prevGoalHand;

    FingerJoint[] fingers;

    [SerializeField]
    float animSpeed = 2f;

    [SerializeField]
    SteamVR_TrackedObject controller;

    // グー
    int[] FingerGoalRock = new int[]{1,1,1,1,1};
    // パー
    int[] FingerGoalPaper = new int[] { 0, 0, 0, 0, 0 };
    // チョキ
    int[] FingerGoalScisser = new int[] { 1, 0, 0, 1, 1 };
    // 指さし
    int[] FingerGoalPoint = new int[] { 1, 0, 1, 1, 1 };

    float[] currentFingerParcent;
    float[] prevFingerParcent;

    Dictionary<HandEnum, int[]> handGoals = new Dictionary<HandEnum, int[]>();

    [SerializeField]
    bool enableTempInput = false;

    List<float> currentParcent;
    List<float> prevParcent;

    private void Start()
    {
        handGoals.Add(HandEnum.Rock, new int[] { 1, 1, 1, 1, 1 });
        handGoals.Add(HandEnum.Scisser, new int[] { 1, 0, 0, 1, 1 });
        handGoals.Add(HandEnum.Paper, new int[] { 0, 0, 0, 0, 0 });
        handGoals.Add(HandEnum.Point, new int[] { 1, 0, 1, 1, 1 });

        currentParcent = new List<float>(4) { 0, 0, 0, 0 };
        prevParcent = new List<float>(4) { 0, 0, 0, 0 };

        currentFingerParcent = new float[] { 0, 0, 0, 0, 0 };
        prevFingerParcent = new float[] { 0, 0, 0, 0, 0 };

        fingers = transform.GetComponentsInChildren<FingerJoint>();

        foreach( FingerJoint f in fingers)
        {
            f.defaultRot = f.transform.localRotation;
        }

        // 最初はパー
        SetHand(HandEnum.Paper);
    }

    // Update is called once per frame
    void Update ()
    {
        var device = SteamVR_Controller.Input((int)controller.index);
        if (device == null) return;

        var triggered = device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
        var gripped = device.GetPress(SteamVR_Controller.ButtonMask.Grip);

        if( enableTempInput )
        {
            gripped = Input.GetKey(KeyCode.H);
        }

        if ( gripped )
        {
            currentGoalHand = HandEnum.Rock;
        }
        else if( triggered )
        {
            currentGoalHand = HandEnum.Point;
        }
        else
        {
            currentGoalHand = HandEnum.Paper;
        }

        if( prevGoalHand != currentGoalHand)
        {
            for( int i=0; i<prevFingerParcent.Length; ++i)
            {
                prevFingerParcent[i] = handGoals[prevGoalHand][i] * prevParcent[(int)prevGoalHand];
            }
        }

        prevGoalHand = currentGoalHand;

        for ( int i=0; i< currentParcent.Count; ++i )
        {
            prevParcent[i] = currentParcent[i];
        }

        for (int i = 0; i < currentParcent.Count; ++i)
        {
            bool gain = i == (int)currentGoalHand;
            {
                currentParcent[i] += animSpeed * Time.deltaTime * (gain ? 1f : -1f);
            }

            currentParcent[i] = Mathf.Clamp(currentParcent[i], 0, 1f);

            // Debug.Log(string.Format("currentParcent[{0}] = {1}", i, currentParcent[i]));
        }

        for ( int i=0; i< currentParcent.Count; ++i)
        {
            if (Mathf.Abs(currentParcent[i] - prevParcent[i]) > 0.001f )
            {
                UpdateHold();
                break;
            }
            // Debug.Log("prev = " + prevParcent[i].ToString());
        }
	}

    /// <summary>
    /// 手の形更新
    /// </summary>
    void UpdateHold()
    {
        var current = currentParcent[(int)currentGoalHand];
        var goal = handGoals[currentGoalHand];

        foreach (FingerJoint f in fingers)
        {
            int fIndex = f.fingerindex;

            //                var value = goal[f.fingerindex] * Mathf.InverseLerp(prevFingerParcent[fIndex], goal[fIndex], current);
            var value = Mathf.InverseLerp(prevFingerParcent[fIndex], goal[fIndex], current);

            float angleTo = f.angle * value;
            f.transform.localRotation = f.defaultRot * Quaternion.Euler(f.axis * angleTo);
        }
    }

    public void SetHandDefault()
    {
        SetHand(HandEnum.Paper);
    }

    /// <summary>
    /// 手の形を更新
    /// １フレームで処理
    /// </summary>
    void SetHand( HandEnum hand )
    {
        var goal = handGoals[hand];

        foreach (FingerJoint f in fingers)
        {
            int fIndex = f.fingerindex;

            //                var value = goal[f.fingerindex] * Mathf.InverseLerp(prevFingerParcent[fIndex], goal[fIndex], current);
            float angleTo = f.angle * goal[fIndex];
            f.transform.localRotation = f.defaultRot * Quaternion.Euler(f.axis * angleTo);
        }
    }
}
