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
            DebugCraeteMsg();
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

        } while (pos.y < 0.2f || distance < minRange);

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

    public void DebugCraeteMsg()
    {
        var msg = new Chat.Msg();

        msg.name = "testName";
        msg.text = "testMsg";

        ReceiveCommentEvent(msg);
    }
}
