using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EggSceneManager : MonoBehaviour
{
    public static EggSceneManager instance;

    string currentSceneName = null;

    int currentSceneIndex = 0;

    [SerializeField]
    string[] sceneNameList;


    const string darkOutSceneName = "DarkOut";

    bool initialized = false;

    bool inTransition = false;

	// Use this for initialization
	void Awake ()
    {
        instance = this;
	}

    private void Start()
    {
        StartCoroutine(InitLoad());
    }

    IEnumerator InitLoad()
    {
        // 暗転シーンをロード
        {
            var loadOperation = SceneManager.LoadSceneAsync(darkOutSceneName, LoadSceneMode.Additive);
            // ロード完了まで待機
            while (loadOperation.isDone) yield return null;
        }

        initialized = true;
    }

    public IEnumerator LoadFirstScene()
    {
        currentSceneIndex = 0;
        yield return LoadScene(sceneNameList[currentSceneIndex]);
    }

    public IEnumerator LoadNextScene()
    {
        currentSceneIndex = (currentSceneIndex + 1) % sceneNameList.Length;
        yield return LoadScene(sceneNameList[currentSceneIndex]);
    }

    public IEnumerator LoadScene( string newSceneName )
    {
        while (!initialized) yield return null;

        if (inTransition) yield break;

        inTransition = true;

        AsyncOperation loadSceneOperation = null;
        AsyncOperation unloadSceneOperation = null;

        // 暗転シーンマネージャが存在しない間待機
        while ( DarkOutManager.instance == null ) yield return null;

        // 暗転
        yield return DarkOutManager.instance.Fade(true);

        // 開いているシーンがあればアンロード
        if( !string.IsNullOrEmpty( currentSceneName ))
        {
            unloadSceneOperation = SceneManager.UnloadSceneAsync(currentSceneName);
        }

        // 次のシーンをロード
        loadSceneOperation = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);

        // ロードとアンロードが完了するまで待機
        while (loadSceneOperation != null && !loadSceneOperation.isDone) yield return null;
        while (unloadSceneOperation != null && !unloadSceneOperation.isDone) yield return null;

        currentSceneName = newSceneName;

        // 新しくロードしたシーンをアクティブにする
        var scene = SceneManager.GetSceneByName(newSceneName);
        SceneManager.SetActiveScene(scene);

        // 暗転明け
        yield return DarkOutManager.instance.Fade(false);

        inTransition = false;
    }

    public IEnumerator ReserveGameObjectToBaseScene<T>( List<T> list )
        where T : MonoBehaviour
    {
        // ベースシーンのロードが完了するまで待機
        var baseScene = SceneManager.GetSceneByName("BaseScene");
        while (!baseScene.isLoaded) yield return null;

        foreach( T g in list)
        {
            SceneManager.MoveGameObjectToScene(g.gameObject, baseScene);
        }
    }
}
