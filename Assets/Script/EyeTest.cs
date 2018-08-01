using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTest : MonoBehaviour {

	[SerializeField]
	Camera cam;
	[SerializeField]
	float camMoveSpeed = 1f;
	float h;
	float v;

	void Update()
	{
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");

		cam.transform.Translate( h * camMoveSpeed * Time.deltaTime, v * camMoveSpeed * Time.deltaTime, 0);
	}
}
