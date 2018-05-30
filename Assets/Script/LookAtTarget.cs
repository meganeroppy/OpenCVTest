using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
	[SerializeField]
	Transform target = null;

	// Update is called once per frame
	void Update ()
	{
		if( target != null )
		{
			transform.LookAt( target );
		}
	}
}
