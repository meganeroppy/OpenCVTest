using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentParts : MonoBehaviour {

    public SkinnedMeshRenderer bodyMesh;
    public SkinnedMeshRenderer eyeMesh;
    public SkinnedMeshRenderer mouthMesh;
    public SkinnedMeshRenderer namePlateMesh;

    [SerializeField]
    Material[] eyeMaterials;
    public Material[] EyeMaterials { get { return eyeMaterials; } }

    [SerializeField]
    Material[] eyeMaterial_smile;
    public Material[] EyeMaterial_smile { get { return eyeMaterial_smile; } }

    [SerializeField]
    Material[] eyeMaterials_surprized;
    public Material[] EyeMaterials_surprized { get { return eyeMaterials_surprized; } }

    [SerializeField]
    Material[] eyeMaterial_sad;
    public Material[] EyeMaterial_sad { get { return eyeMaterial_sad; } }

    [SerializeField]
    Material[] mouthOpenMaterials;
    public Material[] MouthOpenMaterials { get { return mouthOpenMaterials; } }

    [SerializeField]
    Material[] mouthOpenMaterials_negative;
    public Material[] MouthOpenMaterials_negative { get { return mouthOpenMaterials_negative; } }

    [SerializeField]
    Material[] mouthOpenMaterials_surprized;
    public Material[] MouthOpenMaterials_surprized { get { return mouthOpenMaterials_surprized; } }

    [SerializeField]
    Material mouthCloseMaterial_smile;
    public Material MouthCloseMaterial_smile { get { return mouthCloseMaterial_smile; } }

    [SerializeField]
    Material mouthCloseMaterial_negative;
    public Material MouthCloseMaterial_negative { get { return mouthCloseMaterial_negative; } }

    [SerializeField]
    Material mouthCloseMaterial_surprited;
    public Material MouthCloseMaterial_surprited { get { return mouthCloseMaterial_surprited; } }

}
