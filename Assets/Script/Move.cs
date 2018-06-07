using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    [SerializeField]
    float speed = 1;

    [SerializeField]
    Transform target;

    Vector3 targetPos = Vector3.zero;

    bool moving = false;

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.M)) moving = true;

        if (target == null) return;

        targetPos = target.position;

		if( moving )
        {
            var maxMovePerFrame = speed * Time.deltaTime;
            var distance = Vector3.Distance(transform.position, targetPos);
            if (distance <= maxMovePerFrame)
            {
                transform.position = targetPos;
                moving = false;
            }
            else
            {
                var direction = (targetPos - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
        }
	}
}
