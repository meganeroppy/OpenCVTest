using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EggSystem;

public class EggController : MonoBehaviour
{
    // システム関連

    int id;
    public int Id { get { return id; } }

    EyeType myEyeType = EyeType.A;

    MouthType myMouthType = MouthType.A;

    Facial currentFacial = Facial.Normal;

    [SerializeField]
	Transform eggModelTrans;

    /// <summary>
    /// たまごの柄
    /// </summary>
    [SerializeField]
    MeshRenderer eggPettern;

	[SerializeField]
	MeshFilter eggMeshFilter;

    [SerializeField]
    SkinnedMeshRenderer namePlate;

    [SerializeField]
    SpriteRenderer eye;

    [SerializeField]
    SpriteRenderer mouth;

    [SerializeField]
    SpriteRenderer emote;

    /// <summary>
    /// 中身
    /// </summary>
    [SerializeField]
    Content myContent = null;

    [SerializeField]
    GameObject spriteBase = null;

    [SerializeField]
    AudioSource audioSource = null;

    [SerializeField]
    ExplodeShell shellPrefab;

    [SerializeField]
    GameObject hatchEffect;

    [SerializeField]
    GameObject secludeEffect;

    [SerializeField]
    GameObject changeEffect;

    /// <summary>
    /// たまご柄リスト
    /// </summary>
    [SerializeField]
    Material[] eggPetternMaterials;

    /// <summary>
    /// たまごメッシュリスト
    /// </summary>
    [SerializeField]
	Mesh[] eggMeshs;

    /// <summary>
    /// たまご柄回転差分リスト
    /// </summary>
    [SerializeField]
	float[] eggModelRotYOffsets;

    /// <summary>
    /// 名札マテリアルリスト
    /// </summary>
    [SerializeField]
    Material[] namePlateMaterials;
    public Material[] NamePlateMaterials { get { return namePlateMaterials; } }

    /// <summary>
    /// 現在使用中の目
    /// </summary>
    Sprite[] currentEyeSprites;

    /// <summary>
    /// 通常目A
    /// </summary>
    [SerializeField]
    Sprite[] eyeSprites_a;

    /// <summary>
    /// 通常目B
    /// </summary>
    [SerializeField]
    Sprite[] eyeSprites_b;

    /// <summary>
    /// 笑顔目
    /// </summary>
    [SerializeField]
    Sprite[] eyeSprite_smile;

    /// <summary>
    /// 驚き目
    /// </summary>
    [SerializeField]
    Sprite[] eyeSprites_surprized;

    /// <summary>
    /// 悲しみ目
    /// </summary>
    [SerializeField]
    Sprite[] eyeSprite_sad;

    /// <summary>
    /// ひび割れメッシュ
    /// </summary>
    [SerializeField]
    MeshRenderer crackMesh;

    [SerializeField]
    Material[] crackMaterials;

    /// <summary>
    /// 使用中の開き口
    /// </summary>
    Sprite[] currentMouthOpenSprites;

    /// <summary>
    /// 通常開き口A
    /// </summary>
    [SerializeField]
    Sprite[] mouthOpenSprites_a;

    /// <summary>
    /// 通常開き口B
    /// </summary>
    [SerializeField]
    Sprite[] mouthOpenSprites_b;

    /// <summary>
    /// 泣き開き口
    /// </summary>
    [SerializeField]
    Sprite[] mouthOpenSprites_cry;

    /// <summary>
    /// 驚き開き口
    /// </summary>
    [SerializeField]
    Sprite[] mouthOpenSprites_surprized;

    /// <summary>
    /// 使用中閉じ口
    /// </summary>
    Sprite currentMouthCloseSprite;

    /// <summary>
    /// 笑顔閉じ口
    /// </summary>
    [SerializeField]
    Sprite mouthCloseSprite_smile;

    /// <summary>
    /// 泣き閉じ口
    /// </summary>
    [SerializeField]
    Sprite mouthCloseSprite_cry;

    /// <summary>
    /// 驚き閉じ口
    /// </summary>
    [SerializeField]
    Sprite mouthCloseSprite_surprited;

    [SerializeField]
    Sprite[] emoteSprites;

    [HeaderAttribute("まばたき")]
    
    [SerializeField]
    float blinkIntervalMin = 2f;
    [SerializeField]
    float blinkIntervalMax = 5f;

    float blinkInterval;

    float blinkTimer = 0;

    Coroutine blinkCoroutine = null;

    [SerializeField]
    GameObject tracker = null;

    public Vector3 trackerOffetPos = Vector3.zero;

    public Vector3 trackerOffetRot = Vector3.zero;

    [SerializeField]
	bool initializeSelf = false;

	[SerializeField]
	bool enableTempInput = false;

	[SerializeField]
	bool autoRotate = false;

    [SerializeField]
    bool autoTalk = false;

    public bool talking = false;

    bool mouthClose = true;

    float mouthChangeTimer = 0;

    [SerializeField]
    float mouthChangeInterval = 0.25f;

    [SerializeField]
    float hatchDur = 3f;

    bool inHatchingPhase = false;

    bool hatched = false;

    [SerializeField]
    int selfInitializedId = 0;

    [SerializeField]
    bool ignoreTracking = false;

    public bool enableSpecialContent = false;

    public System.Action specialHatchEvent = null;
    public System.Action specialUnhatchEvent = null;

    // Use this for initialization
    void Start()
    {
        // 初期化
        if (initializeSelf)
        {
            Init(selfInitializedId);
        }
    }

    public void Init( int id=0, int birthOrder=0 )
    {
		UpdateEggType( id );

        // 初期状態の顔にする
        currentMouthCloseSprite = mouthCloseSprite_smile;
        if( mouth != null)
            mouth.sprite = currentMouthCloseSprite;

        // 初回まばたきまでの時間を定義
        blinkTimer = 0;
        blinkInterval = Random.Range(blinkIntervalMin, blinkIntervalMax);

        // エモート非表示
		if( emote != null )
	        emote.enabled = false;

        // ひび割れ非表示
        if (crackMesh != null)
        {
            crackMesh.enabled = false;
            crackMesh.material.SetFloat("_Cutoff", 1);
        }

        // トラッキング先のオブジェクトをセット
        AssignToTracker( birthOrder );
    }

    private void Update()
    {
        UpdateBlinkTimer();
        UpdateMouth();
        UpdateTransform();
		UpdateTempInput();

		if( autoRotate ) transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }

	void UpdateTempInput()
	{
		if( !enableTempInput) return;

		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			UpdateEggTypeForward(false);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			UpdateEggTypeForward(true);
		}
        if (Input.GetKeyDown(KeyCode.H))
        {
            Hatch();
        }
    }

    void UpdateBlinkTimer()
    {
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            // 次のまばたきまでの時間をセット
            blinkTimer = 0;
            blinkInterval = Random.Range(blinkIntervalMin, blinkIntervalMax);

            // 時々2連続
            int repeat = Random.Range(0, 5) == 0 ? 2 : 1;

            blinkCoroutine = StartCoroutine(Blink(repeat));
        }
    }

    void UpdateTransform()
    {
        if (!tracker) return;

        transform.SetPositionAndRotation(tracker.transform.position + trackerOffetPos, tracker.transform.rotation * Quaternion.Euler( trackerOffetRot ));
    }

    /// <summary>
    /// まばたき
    /// </summary>
    IEnumerator Blink(int repeat = 1 )
    {
        if (currentEyeSprites == null) yield break;

        if( currentEyeSprites.Length < 2)
        {
            Debug.LogWarning(" 現在の目はまばたきなし ");
            yield break;
        }

		if( eye == null) yield break;

        int count = 0;

        while (count < repeat)
        {
            // 目をつぶる
            if (hatched && myContent.gameObject.activeSelf)
            {
                myContent.Blink(1);
            }
            else
            {
                eye.sprite = currentEyeSprites[1];
            }

            yield return new WaitForSeconds(0.25f);

            // 目を開ける
            if (hatched && myContent.gameObject.activeSelf)
            {
                myContent.Blink(0);
            }
            else
            {
                eye.sprite = currentEyeSprites[0];
            }

            yield return new WaitForSeconds(0.25f);

            count++;
        }

        blinkCoroutine = null;
    }


    /// <summary>
    /// 口パク更新
    /// </summary>
    void UpdateMouth()
    {
        if (!talking && !autoTalk)
        {
			mouthChangeTimer += Time.deltaTime;
			if (mouthChangeTimer < mouthChangeInterval) return;

            if (hatched && myContent.gameObject.activeSelf)
            {
                myContent.UpdateMouth(false, 0);
            }
            else
            {
            if (!mouthClose)
                mouth.sprite = currentMouthCloseSprite;
           }

			// 次回口パク状態になったら即口が開いている状態になるようにする
			mouthChangeTimer = mouthChangeInterval;
			mouthClose = true;
        }
        else
        {
            mouthChangeTimer += Time.deltaTime;
            if (mouthChangeTimer < mouthChangeInterval) return;

            mouthChangeTimer = 0;

            if (mouthClose)
            {
                if (hatched && myContent.gameObject.activeSelf)
                {
                    myContent.UpdateMouth(true, 0);
                }
                else
                {
                    var newMouthSprite = currentMouthOpenSprites.Length == 1 ? currentMouthOpenSprites[0] : currentMouthOpenSprites[Random.Range(0, 2)];
                    mouth.sprite = newMouthSprite;
                }
            }
            else
            {
                if (hatched && myContent.gameObject.activeSelf)
                {
                    myContent.UpdateMouth(false, 0);
                }
                else
                {
                    mouth.sprite = currentMouthCloseSprite;
                }
            }

            mouthClose = !mouthClose;

            // 次のフレーム開始時に再度trueになっていなければ口パク終了
            talking = false;
        }
    }

    /// <summary>
    /// 対応するトラッカーに追従するようにする
    /// </summary>
    public void AssignToTracker( int order )
    {
        if (ignoreTracking) return;

            StartCoroutine(ExecAssignToTracker(order));
    }

    IEnumerator ExecAssignToTracker( int order )
    {
        var objectName = "Tracker" + (order+1).ToString();

        // 見つけるまで10秒間隔でFind
        int searchInterval = 10;

        while( !tracker )
        {
            tracker = GameObject.Find(objectName);

            yield return new WaitForSeconds(searchInterval);
        }
    }

    /// <summary>
    /// 対応するトラッカーに追従するようにする
    /// </summary>
    public void AssignToTracker( GameObject target )
    {
        if (ignoreTracking) return;

        tracker = target;
    }

    void UpdateEggType(int id)
    {
        // たまご柄セット
        if (eggPettern != null)
        {
            eggPettern.material = eggPetternMaterials[id % eggPetternMaterials.Length];
        }

        // たまごメッシュセット
        if (eggMeshFilter != null)
        {
            eggMeshFilter.mesh = eggMeshs[id % eggMeshs.Length];
        }

        // たまご回転差分セット
        if (eggModelTrans != null)
        {
            eggModelTrans.localRotation = Quaternion.Euler(0, eggModelRotYOffsets[id % eggModelRotYOffsets.Length], 0);
        }

		// 名札セット
		if( namePlate != null)
			namePlate.material = namePlateMaterials[ id % namePlateMaterials.Length ];

        // 目タイプセット
        myEyeType = id % 2 == 0 ? EyeType.A : EyeType.B;

        // 通常顔の時だけ目のスプライト差し替え
        if (currentFacial == Facial.Normal)
        {
            currentEyeSprites = myEyeType == EyeType.A ? eyeSprites_a : eyeSprites_b;
            eye.sprite = currentEyeSprites[0];
        }

        // 口タイプセット
        myMouthType = id % 2 == 0 ? MouthType.A: MouthType.B;
        currentMouthOpenSprites = myMouthType == MouthType.A ? mouthOpenSprites_a : mouthOpenSprites_b;

        // ゲームオブジェクト名更新
        if( eggPettern != null)
            gameObject.name = eggPettern.material.name;

        this.id = id;
	}

    /// <summary>
    /// たまごの柄変更
    /// 孵化後の時は中身の種類を変える
    /// </summary>
    /// <param name="forward"></param>
    public void UpdateEggTypeForward(bool forward)
    {
        // エフェクト
        var efs = Instantiate(changeEffect);
        efs.transform.position = transform.position;

        if (hatched)
        {
            myContent.GoToNext(forward);

        }
        else
        {
            int newId = forward ? ((id + 1) % eggPetternMaterials.Length) : ((id + eggPetternMaterials.Length - 1) % eggPetternMaterials.Length);

            UpdateEggType(newId);
        }
    }

    public void SetFacial( Facial newFaicial )
    {
        currentFacial = newFaicial;

        if (hatched)
        {
            // 孵化済みの場合

            myContent.SetFacial(newFaicial);
        }
        else
        {
            // たまごの場合

            // 目のタイプを更新
            switch (currentFacial)
            {
                case Facial.Normal:
                    currentEyeSprites = myEyeType == EyeType.A ? eyeSprites_a : eyeSprites_b;
                    currentMouthOpenSprites = myMouthType == MouthType.A ? mouthOpenSprites_a : mouthOpenSprites_b;
                    currentMouthCloseSprite = mouthCloseSprite_smile;
                    break;
                case Facial.Angry:
                    currentEyeSprites = eyeSprites_surprized;
                    currentMouthOpenSprites = mouthOpenSprites_surprized;
                    currentMouthCloseSprite = mouthCloseSprite_surprited;
                    break;
                case Facial.Sad:
                    currentEyeSprites = eyeSprite_sad;
                    currentMouthOpenSprites = mouthOpenSprites_cry;
                    currentMouthCloseSprite = mouthCloseSprite_cry;
                    break;
                case Facial.Smile:
                    currentEyeSprites = eyeSprite_smile;
                    currentMouthOpenSprites = myMouthType == MouthType.A ? mouthOpenSprites_a : mouthOpenSprites_b;
                    currentMouthCloseSprite = mouthCloseSprite_smile;
                    break;

                default:
                    break;
            }

            // 更新された表情セットでデフォルトの表情
            eye.sprite = currentEyeSprites[0];
            mouth.sprite = currentMouthCloseSprite;
        }

        if( blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }

    public void Hatch( ContentType type=ContentType.Prin )
    {
        if (hatched) return;

        // ひび割れを表示
        StartCoroutine(ExecHatch(type));
    }

    [SerializeField]
    bool newType = true;

    [SerializeField]
    bool withSceneChange = true;

    IEnumerator ExecHatch(ContentType type)
    {
        if (inHatchingPhase) yield break;

        inHatchingPhase = true;

        if (withSceneChange)
        {
            yield return EggSceneManager.instance.LoadScene("Kirakira");

            yield return new WaitForSeconds(1);
        }

        // 効果音
        StartCoroutine(PlayCrackSound());

        // ひび割れ有効に
        crackMesh.gameObject.SetActive(true);
        crackMesh.enabled = true;

        var cutoffParam = "_Cutoff";

        crackMesh.material.SetFloat(cutoffParam, 1);

        float progress = 0;

        float perFrame = 1f / hatchDur;

        // 徐々に割れていく
        if (newType)
        {
            crackMesh.material = crackMaterials[0];

            yield return new WaitForSeconds(1);

            StartCoroutine(PlayCrackSound());
            crackMesh.material = crackMaterials[1];

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(PlayCrackSound());
            crackMesh.material = crackMaterials[2];

            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            while (progress < 1)
            {
                progress += perFrame * Time.deltaTime;

                float val = 1 - progress;
                if (val < 0.01f) val = 0.01f;
                crackMesh.material.SetFloat(cutoffParam, val);

                yield return null;
            }
        }

        // ひび割れ終了
        Debug.Log("ひびわれ終わり");

        // ここから中身が登場

        // エフェクト
        var efs = Instantiate(hatchEffect);
        efs.transform.position = transform.position;

        // 殻飛散オブジェクト取得
        var exp = ExplodeShellManager.instance;
        if( exp != null )
        {
            var shell = exp.GetOne();

            shell.transform.SetPositionAndRotation(transform.position, transform.rotation);
            shell.transform.localScale = transform.localScale;
            shell.Execute(eggPetternMaterials[id]);
        }

        if (enableSpecialContent)
        {
            if (specialHatchEvent != null) specialHatchEvent();
        }
        else
        {
            myContent.gameObject.SetActive(true);
            // 見た目をセット
            myContent.Set((ContentType)(id % ((int)ContentType.Count)), id);
        }

        // たまご部分を非表示
        eggPettern.gameObject.SetActive(false);
        crackMesh.gameObject.SetActive(false);
        spriteBase.gameObject.SetActive(false);
        namePlate.gameObject.SetActive(false);

        inHatchingPhase = false;

        hatched = true;

        if( withSceneChange )
        {
            yield return new WaitForSeconds(3f);

            yield return EggSceneManager.instance.LoadScene("Classroom");
        }
    }

    /// <summary>
    /// 卵に戻る
    /// </summary>
    public void Seclude()
    {
        if (!hatched) return;

        // エフェクト
        var efs = Instantiate(secludeEffect);
        efs.transform.position = transform.position;

        if (enableSpecialContent)
        {
            if (specialUnhatchEvent != null) specialUnhatchEvent();
        }
        else
        {
            // 中身を非表示
            myContent.gameObject.SetActive(false);
        }

        // たまご部分を表示
        eggPettern.gameObject.SetActive(true);
        crackMesh.gameObject.SetActive(false);
        spriteBase.gameObject.SetActive(true);
        namePlate.gameObject.SetActive(true);

        // 一度オブジェクトを無効になるとクロスが動作しなくなる？
        // コンポーネントを一度無効にして再度有効にすると動くみたいなのでとりあえず対応
        var cloth = namePlate.GetComponent<Cloth>();
        if( cloth != null )
        {
            cloth.enabled = false;
            cloth.enabled = true;
        }

        hatched = false;
    }

    IEnumerator PlayCrackSound( int loopNum=1 )
    {
        int count = 0;
        while (count++ < loopNum)
        {
            audioSource.Play();

            while (audioSource.isPlaying) yield return null;
        }
    }

}
