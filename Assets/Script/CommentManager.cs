using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouTubeLive;
using EggSystem;

public class CommentManager : MonoBehaviour {

	[SerializeField]
	YtlController ctrl;

	[SerializeField]
	AudioSource source;

	[SerializeField]
	CommentBoard commentBoardPrefab;

    [SerializeField]
    float boradLifeTime = 60;

    [SerializeField]
    int presetBoardPoolCount = 30;

    [SerializeField]
    Transform center = null;

    [SerializeField]
    float minRange = 3f;
    [SerializeField]
    float maxRange = 10f;
    [SerializeField]
    float maxHightDif = 0.2f;

    void Start ()
    {
		ctrl.OnMessage += msg => { ReceiveCommentEvent( msg); 
		};

        CreatePresetBoardPool();
	}

    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.Space))
        {
            CraeteTutorialMsgs();
        }
    }

    void ReceiveCommentEvent( Chat.Msg msg )
	{
		Debug.Log(msg.name + ": " + msg.text + " [ " + msg.img + " ] ");

		source.Play();

        var obj = GetBoard();

        Vector3 pos = Vector3.zero;
        float distance = 0;

        do
        {
            pos = center.position + ( Random.insideUnitSphere * maxRange);
            distance = (center.position - pos).magnitude;

        } while ( Mathf.Abs( pos.y - center.position.y) > maxHightDif || distance < minRange  || ExistInBadPosition( pos, center.position ) );

        obj.transform.position = pos;

        var dir = (obj.transform.position - center.position);

    //    obj.transform.rotation = Quaternion.Euler(dir);
        obj.transform.forward = dir;

        obj.Set( msg, boradLifeTime );
	}

    CommentBoard GetBoard()
    {
        if( CommentBoard.pool == null )
        {
            CommentBoard.pool = new List<CommentBoard>();
        }

        var pool = CommentBoard.pool;
        CommentBoard ret = null;
        
        for( int i=0; i< pool.Count; ++i )
        {
            if( !pool[i].gameObject.activeInHierarchy )
            {
                ret = pool[i];
                ret.gameObject.SetActive(true);
                break;
            }
        }

        if( ret == null )
        {
            ret = Instantiate(commentBoardPrefab);
            ret.transform.SetParent(transform);
            pool.Add(ret);
        }

        return ret;
    }

    [SerializeField]
    float frontThreshold = 0.2f;

	/// <summary>
	/// 後ろ半分と正面の真ん中はtrue
	/// 視聴者から見てキャラとかぶるのでチェック
	/// </summary>
    bool ExistInBadPosition(Vector3 pos, Vector3 center)
    {
        // 後ろ半分
		if (pos.z > center.z) return true;

        // 前方正面
		if (Mathf.Abs( center.x - pos.x) < frontThreshold) return true;

		return false;
    }

    void CreatePresetBoardPool()
    {
        if (CommentBoard.pool == null)
        {
            CommentBoard.pool = new List<CommentBoard>();
        }

        var pool = CommentBoard.pool;

        for (int i = 0; i < presetBoardPoolCount; ++i)
        {
            var obj = Instantiate(commentBoardPrefab);
            obj.transform.SetParent(transform);
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    public void CraeteTutorialMsgs()
    {
		// すでに表示されているメッセージを削除
		if( CommentBoard.pool != null )
		{
			for( int i=0 ; i < CommentBoard.pool.Count ; ++i )
			{
				if( CommentBoard.pool[i].gameObject.activeInHierarchy )
				{
					CommentBoard.pool[i].gameObject.SetActive(false);
				}
			}
		}

		if( tutorialCo != null ) 
		{
			StopCoroutine( tutorialCo );
			tutorialCo = null;
		}

		tutorialCo = StartCoroutine( ExecCreateMsgs() );
    }

	[SerializeField]
	float tutorialMsgInterval = 8f;

	[SerializeField]
	string[] tutorialMsgs = new string[]
	{
		"こんにちは！バーチャルユーチューバーになった気分はどうですか？",
		"たくさんしゃべってみるのが盛り上げるコツです。",
		"早速大きな声で言ってみましょう。「こんにちは！」",
		"これからチュートリアルを開始します！まず、今見えているものについて説明します。",
		"目の前に映っているのはWebカメラの映像、その上に表示された映像が、今ライブ配信されている映像です。",
		"配信を見ているみんなに手を振ってみましょう！やっほ〜。",
		"コントローラの操作について説明します。使うのは、タッチパッドとトリガーです。",
		"手元を見てください。顔アイコンが４つある部分がタッチパッド、人指し指が乗っているのがトリガーです。",
		"タッチパッドの顔アイコン部分を押し込むと、表情を変えることができます。",
		"笑ってみてください！タッチパッドの下を押し込むと笑顔になります。スマイル！",
		"トリガーを引くと、手をにぎらせることができます。",
		"このボードに触った状態で、手をにぎってみてください！つかむことができます！",
		"もう気づいているかもしれませんが、このボードには視聴者のみんなからのコメントも表示されます！",
		"リアクションをして、視聴者と交流してみましょう！",
		"以上でチュートリアルを終了します。わからないことがあったら、お気軽にスタッフを呼んでください！",
		"良いバーチャルユーチューバー体験を！",
	};

	Coroutine tutorialCo = null;

	IEnumerator ExecCreateMsgs()
	{
		var msg = new Chat.Msg();

		msg.name = "チュートリアル";

		for( int i=0 ; i< tutorialMsgs.Length ; ++i )
		{
			msg.text = tutorialMsgs[i];

			ReceiveCommentEvent(msg);

			yield return new WaitForSeconds( tutorialMsgInterval );
		}

		tutorialCo = null;
	}
}
