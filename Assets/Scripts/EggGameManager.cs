using System.Collections;
using System.Collections.Generic;
using EggSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  ゲーム全般の処理
/// </summary>
public class EggGameManager : MonoBehaviour {

    public static EggGameManager instance;

    [SerializeField]
    float eggScale = 1f;

    [SerializeField]
    EggController eggPrefab;

    [SerializeField]
    int[] eggIdList;

    [SerializeField]
    Transform cameraPrefab;

    int currentCameraIndex = 0;
    public int CurrentCameraIndex{ get{ return currentCameraIndex; } }

    List<Camera> cameraList = new List<Camera>();
    public List<Camera> CameraList { get { return cameraList; } }

    List<EggController> eggList = new List<EggController>();
    public List<EggController> EggList { get { return eggList; } }
    
    /// <summary>
    /// 仮選択インデックス
    /// Touchコントローラで使用
    /// </summary>
    int tempSelectIndex = 0;
    public int TempSelectIndex { get { return tempSelectIndex; } }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(OnLoad());
    }

    IEnumerator OnLoad()
    {
        // シーンマネージャが存在するまで待機
        while (EggSceneManager.instance == null) yield return null;

        yield return EggSceneManager.instance.LoadFirstScene();

        // カメラリスト更新
        yield return UpdateCameras();

        // たまごを生成
        int birthOrder = 0;
        foreach( int e in eggIdList)
        {
            var egg = Instantiate(eggPrefab);
            egg.Init(e, birthOrder++);
			egg.transform.localScale = Vector3.one * eggScale;
            eggList.Add(egg);
        }

        // 生成したたまごをベースシーンに移動
       yield return EggSceneManager.instance.ReserveGameObjectToBaseScene<EggController>(eggList);
    }

    IEnumerator UpdateCameras()
    {
        while (CameraPositionManager.instance == null)
        {
            yield return null;
        }

        // 現在のリストをクリア
        cameraList.Clear();

        var c = CameraPositionManager.instance;

        foreach (Camera t in c.Cameras)
        {
            cameraList.Add(t);
        }

        SwitchCamera(true);
    }

    /// <summary>
    /// カメラの仮選択インデックス切り替え
    /// </summary>
    public void SetCameraTempIndex(bool forward)
    {
        if (cameraList == null)
            return;

        if (forward)
            tempSelectIndex = (tempSelectIndex + 1) % cameraList.Count;
        else
            tempSelectIndex = (tempSelectIndex + cameraList.Count - 1) % cameraList.Count;
    }

    /// <summary>
    /// カメラ切り替え
    /// </summary>
    public void SwitchCamera(bool forward)
    {
        if (cameraList == null)
            return;

        if (forward)
            currentCameraIndex = (currentCameraIndex + 1) % cameraList.Count;
        else
            currentCameraIndex = (currentCameraIndex + cameraList.Count - 1) % cameraList.Count;

        SetActiveCamera(currentCameraIndex);
    }

    [SerializeField]
    ViewingDirection viewScript = null;

    public void SetActiveCamera(int newIndex)
    {
        if (newIndex >= cameraList.Count) return;

        currentCameraIndex = newIndex;

        // 仮選択インデックスを更新
        tempSelectIndex = currentCameraIndex;

        for (int i = 0; i < cameraList.Count; ++i)
        {
            bool flag = i == currentCameraIndex;

            cameraList[i].enabled = flag;

            var listener = cameraList[i].GetComponent<AudioListener>();
            if (listener != null) listener.enabled = flag;

            var sprite = cameraList[i].GetComponentInChildren<SpriteRenderer>();
            if( sprite != null )
            {
                var color = sprite.color;
                color.a = flag ? 1 : 0.5f;
                sprite.color = color;
            }

            if ( flag )
            {
                if (viewScript != null)
                {
                    viewScript.lookTarget = cameraList[i].transform;
                }
                else
                {
                    Debug.LogWarning("視線ターゲットがnull");
                }
            }

            if (cameraList[i].transform.childCount <= 0)
            {
                Debug.LogWarning(cameraList[i].name + "は子要素なし");
                continue;
            }
            var efsCam = cameraList[i].transform.GetChild(0).GetComponent<Camera>();

            if (efsCam == null)
            {
                Debug.LogWarning(efsCam.name + "にカメラコンポーネントなし");
                continue;
            }
            efsCam.enabled = flag;
        }

        UpdateUi();
    }

    public UnityEngine.UI.Text camName;

    void UpdateUi()
    {
        var currentCam = cameraList[currentCameraIndex];
        camName.text = currentCam.name;
    }

    [SerializeField]
    string effectTargetMaskName = "Egg";

    public void SetEffect(Emote emote)
    {
        // 正面のたまごを取得
        var hit = GetCenterObject();
        if (hit.Equals(default(RaycastHit))) return;

        GameObject obj = null;
        Vector3 position = hit.collider.transform.position;

        if (emote == Emote.Happy)
        {
            obj = Instantiate(happy);
        }

        else if (emote == Emote.Angry)
        {
            obj = Instantiate(angry);
        }

        else if (emote == Emote.Sad)
        {
            obj = Instantiate(sad);
        }

        else if (emote == Emote.Joy)
        {
            obj = Instantiate(joy);
        }

        obj.transform.position = position;
        obj.transform.forward = cameraList[currentCameraIndex].transform.forward;

    }

	public void SetEggPettern(bool forward)
	{
        // 正面のたまごを取得
        var hit = GetCenterObject();
        if (hit.Equals( default(RaycastHit))) return;
        
		// 柄変更
		var egg = hit.transform.GetComponent<EggController>();
		if( egg != null )
		{
			egg.UpdateEggTypeForward( forward );
		}
	}

    public void SetEggPettern(bool forward, int index)
    {
        if (eggList == null || index >= eggList.Count) return;

        var egg = eggList[index];

        if (egg != null)
        {
            egg.UpdateEggTypeForward(forward);
        }
    }

    public void SetEggFacial( Facial newFacial )
    {
        // 正面のたまごを取得
        var hit = GetCenterObject();
        if (hit.Equals(default(RaycastHit))) return;

        var egg = hit.transform.GetComponent<EggController>();
        if (egg != null)
        {
            egg.SetFacial(newFacial);
        }
    }

    public void SetEggFacial(Facial newFacial, int index)
    {
        if (eggList == null || index >= eggList.Count) return;

        var egg = eggList[index];
        if (egg != null)
        {
            egg.SetFacial(newFacial);
        }
    }

    /// <summary>
    /// 孵化する
    /// toHatchがfalseの時は卵に戻る
    /// </summary>
    /// <param name="toHatch"></param>
    /// <param name="index"></param>
    public void SetEggHatch(bool toHatch, int index)
    {
        if (eggList == null || index >= eggList.Count) return;

        var egg = eggList[index];

        if (egg != null)
        {
            if (toHatch)
            {
                egg.Hatch();
            }
            else
            {
                egg.Seclude();
            }
        }
    }


    /// <summary>
    /// しゃべりモード切替
    /// </summary>
    public void SetEggTalk()
    {
        // 正面のたまごを取得
        var hit = GetCenterObject();
        if (hit.Equals(default(RaycastHit))) return;

        var egg = hit.transform.GetComponent<EggController>();
        if (egg != null)
        {
            egg.talking = !egg.talking;
        }
    }

    /// <summary>
    /// しゃべりモードセット
    /// </summary>
    public void SetEggTalk(int index)
    {
        if (eggList == null || index >= eggList.Count) return;

        var egg = eggList[index];
        if (egg != null)
        {
            egg.talking = true;
        }
    }

    /// <summary>
    /// 現在のカメラから見て正面のオブジェクトを取得
    /// レイヤーがEggのもののみ
    /// </summary>
    RaycastHit GetCenterObject()
    {
        if (cameraList == null || cameraList.Count < 1) return default(RaycastHit);

        int maskNo = LayerMask.NameToLayer(effectTargetMaskName);
        int mask = 1 << maskNo;
        var currentCam = cameraList[currentCameraIndex];

        RaycastHit hit;
        if (!Physics.Raycast(currentCam.transform.position, currentCam.transform.forward, out hit, 10f, mask))
        {
            Debug.Log("ヒットなし");
            return default(RaycastHit);
        }

        Debug.Log(hit.transform.gameObject.name + "にヒット");

        return hit;
    }

    [SerializeField]
    GameObject happy;

    [SerializeField]
    GameObject angry;

    [SerializeField]
    GameObject sad;

    [SerializeField]
    GameObject joy;

    public void AssignEggToTrackers()
    {
        if (eggList == null || eggList.Count < 1) return;

        foreach( EggController e in eggList )
        {
            e.AssignToTracker( eggList.IndexOf( e ) );
        }
    }

    public IEnumerator GotoNextScene()
    {
        var sm = EggSceneManager.instance;
        if (sm == null) yield break;

        yield return sm.LoadNextScene();
    }
}
