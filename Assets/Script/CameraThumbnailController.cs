using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraThumbnailController : MonoBehaviour {

    Outline[] outlineList;

    int currentActiveThumbnailIndex = 0;
    int currentTempSelectThumbnailIndex = 0;

    // Use this for initialization
    void Start ()
    {
        // アウトラインを取得
        outlineList = transform.GetComponentsInChildren<Outline>();

        StartCoroutine(OnLoad());
	}

    IEnumerator OnLoad()
    {
        while (EggGameManager.instance == null) yield return null;

        currentActiveThumbnailIndex = EggGameManager.instance.TempSelectIndex;
    }

	// Update is called once per frame
	void Update ()
    {
        while (EggGameManager.instance == null) return;

        if (currentActiveThumbnailIndex != EggGameManager.instance.CurrentCameraIndex)
        {
            currentActiveThumbnailIndex = EggGameManager.instance.CurrentCameraIndex;
            UpdateThumbnail();
        }

        if (currentTempSelectThumbnailIndex != EggGameManager.instance.TempSelectIndex)
        {
            currentTempSelectThumbnailIndex = EggGameManager.instance.TempSelectIndex;
            UpdateThumbnail();
        }
    }

    void UpdateThumbnail()
    {
        for( int i=0; i < outlineList.Length; ++i )
        {
            if (i == currentActiveThumbnailIndex || i == currentTempSelectThumbnailIndex)
            {
                outlineList[i].enabled = true;
                outlineList[i].effectColor = i == currentActiveThumbnailIndex ? Color.black : Color.yellow;
            }
            else
            {
                outlineList[i].enabled = false;
            }
        }
    }
}
