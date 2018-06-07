using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エフェクトを生成して管理
/// </summary>
public class EffectMaker : MonoBehaviour {

    public static EffectMaker instance; 

    [SerializeField]
    ParticleSystem effectAudiencePrefab = null;

    [SerializeField]
    ParticleSystem effectTuberPrefab = null;

    [SerializeField]
    Vector3 minAudience;

    [SerializeField]
    Vector3 maxAudience;

    [SerializeField]
    Vector3 minTuber;

    [SerializeField]
    Vector3 maxTuber;

    private void Awake()
    {
        instance = this;
    }

    public void Make( Vector3 posAudience=default(Vector3), Vector3 posTuber = default(Vector3))
    {
        // 来場者向けエフェクト 
        {
            var obj = Instantiate(effectAudiencePrefab);
            obj.transform.position = posAudience;

        }

        // 実況者向けエフェクト 
        {
            var obj = Instantiate(effectTuberPrefab);
            obj.transform.position = posTuber;
        }

    }

    private void Update()
    {
        // テストでキーボード入力
        if( Input.GetKeyDown(KeyCode.F) )
        {
            var posAudience = new Vector3(Random.Range(minAudience.x, maxAudience.x), Random.Range(minAudience.y, maxAudience.y), Random.Range(minAudience.z, maxAudience.z));

            var posTuber = new Vector3(Random.Range(minTuber.x, maxTuber.x), Random.Range(minTuber.y, maxTuber.y), Random.Range(minTuber.z, maxTuber.z));

            Make(posAudience, posTuber);
        }
    }
}
