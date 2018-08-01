using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour {

	/// <summary>
	/// 時針
	/// </summary>
	[SerializeField]
	Transform hour;
		
	/// <summary>
	/// 分針
	/// </summary>
	[SerializeField]
	Transform minute;

	/// <summary>
	/// 秒針
	/// </summary>
	[SerializeField]
	Transform second;

	System.DateTime now;

	float rot;

	const float HOUR_MAX = 12f;
	const float MINUTE_MAX = 60f;
	const float SECOND_MAX = 60f;

	const float ROT_MAX = 360f;

	[SerializeField]
	float timeDiffHour = 0;

	[SerializeField]
	bool log = false;

	const float Interval = 1f;
	float timer = 0;

	// Update is called once per frame
	void Update ()
	{
		timer += Time.deltaTime;
		if( timer < Interval ) return;

		timer = 0;

		now = System.DateTime.Now;

		if( Mathf.Abs(timeDiffHour) > 0 )
			now = now.AddHours( (double)timeDiffHour );

		var h = now.Hour % HOUR_MAX;
		var m = now.Minute % MINUTE_MAX;		
		var s = now.Second % SECOND_MAX;

		rot = 
			(ROT_MAX / (HOUR_MAX / h)) 
			+
			(ROT_MAX / HOUR_MAX) * ( m / MINUTE_MAX);
		hour.localRotation = Quaternion.Euler(Vector3.forward * rot);

		rot = 
			(ROT_MAX / (MINUTE_MAX / m))
			+ 
			(ROT_MAX / MINUTE_MAX) * ( s / SECOND_MAX);
		minute.localRotation = Quaternion.Euler(Vector3.forward * rot);

		rot =
			(ROT_MAX / (SECOND_MAX / s));
		second.localRotation = Quaternion.Euler(Vector3.forward * rot);

		if( log )
			Debug.Log( System.DateTime.Now );	
	}
}
