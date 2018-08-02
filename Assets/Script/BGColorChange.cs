using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGColorChange : MonoBehaviour {

    [SerializeField]
    List<Camera> cameraList;

    BgType current;

    [SerializeField]
    GameObject classroomAsset;

    [SerializeField]
    Material whiteMat;

    [SerializeField]
    Material blackMat;

    enum BgType
    {
        White,
        Black,
        Classroom,
        Count,
    }

	// Use this for initialization
	void Start () {
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
                cameraList.ForEach( c => c.backgroundColor = Color.white);
                RenderSettings.skybox = whiteMat;
                if (classroomAsset.activeInHierarchy) classroomAsset.SetActive(false);
                break;
            case BgType.Black:
                cameraList.ForEach(c => c.backgroundColor = Color.black);
                RenderSettings.skybox = blackMat;
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
