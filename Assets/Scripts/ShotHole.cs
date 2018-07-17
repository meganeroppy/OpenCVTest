using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotHole : MonoBehaviour {

	[SerializeField]
	SpriteRenderer sprite;

	[SerializeField]
	float lifeTime = 1f;

	float timer = 0;

	Color color;

	float alpha;

	// Update is called once per frame
	void Update () 
	{
		if( !gameObject.activeSelf ) return;

		timer += Time.deltaTime;

		color = sprite.color;

		alpha = 1f - (timer / lifeTime);

		color.a = alpha;

		sprite.color = color;

		if( timer >= lifeTime )
		{
			gameObject.SetActive(false);
		}
	}

	void OnEnable()
	{
		timer = 0;
		sprite.color = Color.white;
	}
}
