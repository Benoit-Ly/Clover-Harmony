using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

    RaycastHit hit;

	void Update ()
    {
        Physics.Raycast(transform.root.position, Vector3.down, out hit, float.MaxValue, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);

        transform.position = hit.point;
	}
}
