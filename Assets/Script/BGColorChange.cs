using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGColorChange : MonoBehaviour {

    Camera myCamera;

    BgType current;

    [SerializeField]
    GameObject classroomAsset;

    enum BgType
    {
        White,
        Black,
        Classroom,
        Count,
    }

	// Use this for initialization
	void Start () {
        myCamera = GetComponent<Camera>();
        current = BgType.White;
    }
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.B))
        {
            Change();
        }
	}

    private void Change()
    {
        current = (BgType)((((int)current) + 1) % (int)BgType.Count);

        switch( current )
        {
            case BgType.White:
                myCamera.backgroundColor = Color.white;
                if (classroomAsset.activeInHierarchy) classroomAsset.SetActive(false);
                break;
            case BgType.Black:
                myCamera.backgroundColor = Color.black;
                if (classroomAsset.activeInHierarchy) classroomAsset.SetActive(false);
                break;
            case BgType.Classroom:
                classroomAsset.SetActive(true);
                break;
            default:
                break;
        }

    }
}
