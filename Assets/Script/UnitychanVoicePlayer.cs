using UnityEngine;
using System.Collections;

[RequireComponent( typeof(UnityChanLipSync) )]
public class UnityChanVoicePlayer : MonoBehaviour 
{
	private UnityChanLipSync lipSync_;
	public AudioClip[] audioClips;
	public int index = 0;

	void Start()
	{
		lipSync_ = GetComponent<UnityChanLipSync>();
	}

	void Update() 
	{
		if (Input.anyKeyDown) {
			if (index < 0 || index >= audioClips.Length) index = 0; 
			lipSync_.Play( audioClips[index] );
			++index;
		}
	}
}