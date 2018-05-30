using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {

	[SerializeField]
	float speed;

//	[SerializeField]
//	Vector2 limit = Vector2.one;

	Vector3 pos;

	// Use this for initialization
	void Start () {
		pos = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        pos += Vector3.right * h * speed * Time.deltaTime;
        pos += Vector3.up * v * speed * Time.deltaTime;

//		pos.x = Mathf.Clamp( pos.x, -limit.x, limit.x );
//		pos.y = Mathf.Clamp( pos.y, -limit.y, limit.y );

		transform.position = pos;
    }
}
