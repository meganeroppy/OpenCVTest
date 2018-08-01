using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wine : MonoBehaviour {

	[SerializeField]
	GameObject wineObject;

	[SerializeField]
	GameObject effect;

	public void Set( bool key )
	{
		if( wineObject == null ) return;

		wineObject.SetActive(key);

		if( effect == null ) return;

		Instantiate( effect, wineObject.transform.position, Quaternion.identity );
	}
}
