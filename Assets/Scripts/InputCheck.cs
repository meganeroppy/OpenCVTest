using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCheck : MonoBehaviour {

    string prefix = "joystick button ";
    // Use this for initialization
    void Start (){}
	
	// Update is called once per frame
	void Update ()
    {
        for( int i=0; i< 12; ++i )
        {
            var name = prefix + i.ToString();
            if ( Input.GetKeyDown( name ) )
            {
                Debug.Log(name + "がおされた");
            }

            name = "Trigger";
            var val = Input.GetAxis( name );
            if ( Mathf.Abs( val ) > 0 )
            {
                Debug.Log( name + " : " + val.ToString() );
            }
        }
	}
}
