using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDPParser : MonoBehaviour
{
	[SerializeField]
	UDPReceive source;

	string data = null;

	[HideInInspector]
	public Vector2 parsedData = Vector2.zero;

	// Update is called once per frame
	void Update () 
	{
		if( source == null ) return;

		data = UDPReceive.text;

		if( string.IsNullOrEmpty( data ) ) return;

		// sample = x:669,y:499

		var split1 = data.Split(',');

		if( split1.Length < 2) return;

		for( int i=0 ; i<split1.Length ; ++i )
		{
			var split2 = split1[i].Split(':');

			if( split2.Length < 2 ) continue;

			float result = 0;

			if( i == 0 )
			{
				if( float.TryParse(split2[1], out result) ) parsedData.x = result;
			}
			else if( i == 1 )
			{
				if( float.TryParse(split2[1], out result) ) parsedData.y = result;
			}
		}

		Debug.Log( parsedData );
	}
}
