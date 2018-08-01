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

	void Start () {
		ctrl.OnMessage += msg => { ReceiveCommentEvent( msg); 
		};
	}

	void ReceiveCommentEvent( Chat.Msg msg )
	{
		Debug.Log(msg.name + ": " + msg.text + " [ " + msg.img + " ] ");

		source.Play();

		var obj = Instantiate( commentBoardPrefab );
		obj.Set( msg );
	}

}
