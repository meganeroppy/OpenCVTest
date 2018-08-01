using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EggSystem;

public class Content : MonoBehaviour
{
    [SerializeField]
    ContentParts[] list;

    ContentParts currentContentParts = null;
    int currentIndex = 0;

    [SerializeField]
    EggController master;

    private void Awake()
    {
        UpdateCurrentContent();
    }

    public void GoToNext(bool forward=true)
    {
        if( forward )
        {
            currentIndex = (currentIndex + 1) % list.Length;
        }
        else
        {
            currentIndex = (currentIndex + list.Length - 1) % list.Length;
        }

        UpdateCurrentContent();

        SetFacial(Facial.Normal);

        // 名札セット
        var nameMat = master.NamePlateMaterials[ master.Id % master.NamePlateMaterials.Length];
        currentContentParts.namePlateMesh.material = nameMat;
    }

    public void Set( ContentType type, int id )
    {
        currentIndex = (int)type;

        UpdateCurrentContent();

        // 初期状態の顔にする
//        currentEyeMaterials = currentContentParts.EyeMaterials;
//        currentMouthCloseMaterial = currentContentParts.MouthCloseMaterial_smile;
//        currentContentParts.mouthMesh.material = currentMouthCloseMaterial;

        SetFacial(Facial.Normal);

        // 名札セット
        var nameMat = master.NamePlateMaterials[id % master.NamePlateMaterials.Length];
        currentContentParts.namePlateMesh.material = nameMat;

        // 一度オブジェクトを無効になるとクロスが動作しなくなる？
        // コンポーネントを一度無効にして再度有効にすると動くみたいなのでとりあえず対応
        var cloth = currentContentParts.namePlateMesh.GetComponent<Cloth>();
        if (cloth != null)
        {
            cloth.enabled = false;
            cloth.enabled = true;
        }
    }

    void UpdateCurrentContent()
    {
        for (int i = 0; i < list.Length; ++i)
        {
            bool val = i == currentIndex;
            list[i].gameObject.SetActive(val);
            if (val) currentContentParts = list[i];
        }
    }

    Material[] currentEyeMaterials;

    Material[] currentMouthOpenMaterials;

    Material currentMouthCloseMaterial;

    /// <summary>
    /// まばたき
    /// </summary>
    public void Blink(int key)
    {
        if (currentEyeMaterials.Length < 2)
        {
            Debug.LogWarning(" 現在の目はまばたきなし ");
            return;
        }

        // 目をつぶる
        currentContentParts.eyeMesh.material = currentEyeMaterials[key % currentEyeMaterials.Length];
    }


    /// <summary>
    /// 口パク更新
    /// </summary>
    public void UpdateMouth(bool open, int key)
    {
        if (open)
        {
            var newMouthMaterial = currentMouthOpenMaterials.Length == 1 ? currentMouthOpenMaterials[0] : currentMouthOpenMaterials[Random.Range(0, currentMouthOpenMaterials.Length)];
            currentContentParts.mouthMesh.material = newMouthMaterial;
        }
        else
        {
            currentContentParts.mouthMesh.material = currentMouthCloseMaterial;
        }
    }
    
    public void SetFacial(Facial newFaicial)
    {
        // 目のタイプを更新
        switch (newFaicial)
        {
            case Facial.Normal:
                currentEyeMaterials = currentContentParts.EyeMaterials;
                currentMouthOpenMaterials = currentContentParts.MouthOpenMaterials;
                currentMouthCloseMaterial = currentContentParts.MouthCloseMaterial_smile;
                break;
            case Facial.Angry:
                currentEyeMaterials = currentContentParts.EyeMaterials_surprized;
                currentMouthOpenMaterials = currentContentParts.MouthOpenMaterials_surprized;
                currentMouthCloseMaterial = currentContentParts.MouthCloseMaterial_surprited;
                break;
            case Facial.Sad:
                currentEyeMaterials = currentContentParts.EyeMaterial_sad;
                currentMouthOpenMaterials = currentContentParts.MouthOpenMaterials_negative;
                currentMouthCloseMaterial = currentContentParts.MouthCloseMaterial_negative;
                break;
            case Facial.Smile:
                currentEyeMaterials = currentContentParts.EyeMaterial_smile;
                currentMouthOpenMaterials = currentContentParts.MouthOpenMaterials;
                currentMouthCloseMaterial = currentContentParts.MouthCloseMaterial_smile;
                break;

            default:
                break;
        }

        // 更新された表情セットでデフォルトの表情
        currentContentParts.eyeMesh.material = currentEyeMaterials[0];
        currentContentParts.mouthMesh.material = currentMouthCloseMaterial;
    }

}
