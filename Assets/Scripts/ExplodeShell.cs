using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飛散するたまごの殻
/// </summary>
public class ExplodeShell : MonoBehaviour {

    [SerializeField]
    FracturedObject frac;

    MeshRenderer[] meshes;

    [SerializeField]
    float waitForSmallen = 3;

    [SerializeField]
    float durationSmallen = 5;

    [ContextMenu("Execute")]
    public void Execute()
    {
        Execute(null);
    }

    public void Execute( Material newMat )
    {
        StartCoroutine(Exec(newMat));
    }

    /// <summary>
    /// 一定時間の後に飛散した殻のスケールを徐々に小さくし、スケールが0となったらこのゲームオブジェクトを削除する
    /// </summary>
    IEnumerator Exec(Material newMat)
    {
        meshes = transform.GetComponentsInChildren<MeshRenderer>();

        frac.Explode(transform.position, 80f);

        yield return null;

        // 殻を子要素にする
        for (int i = 0; i < meshes.Length; ++i)
        {
            var m = meshes[i];
            m.transform.SetParent(transform);
            m.enabled = !m.name.Contains("@");

            if (m.materials.Length < 2) continue;

            m.material = newMat;
        }

        yield return new WaitForSeconds(waitForSmallen);

        float progress = 0;
        float valueParFrame = 1f / durationSmallen;

        float scale = 1f;

        while( progress < 1 )
        {
            progress += valueParFrame * Time.deltaTime;

            scale = 1 - progress;

            for ( int i=0; i<meshes.Length; ++i )
            {
                meshes[i].transform.localScale = Vector3.one * scale;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
