using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エフェクトを生成して管理
/// </summary>
public class EffectMaker : MonoBehaviour {

    public static EffectMaker instance; 

    [SerializeField]
    ParticleSystem prefab = null;

    [SerializeField]
    Vector3 min;

    [SerializeField]
    Vector3 max;

    private void Awake()
    {
        instance = this;
    }

    public void Make( Vector3 worldPos=default(Vector3) )
    {
        var obj = Instantiate(prefab);

        obj.transform.position = worldPos;
    }

    private void Update()
    {
        // テストでキーボード入力
        if( Input.GetKeyDown(KeyCode.F) )
        {
            var pos = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
            Make(pos);
        }
    }
}
