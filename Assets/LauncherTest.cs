using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherTest : MonoBehaviour {

    Vector3 origin;
    Vector3 lastPosition;
    Vector3 newPosition;
    [SerializeField]
    float maxTime = 2f;
    float t = 0f;

    [SerializeField]
    float angle;
    float radAngle;
    [SerializeField]
    float speed;
    [SerializeField]
    float velocity;
    [SerializeField]
    float height;

	// Use this for initialization
	void Start () {
        origin = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {

        if (t >= maxTime)
        {
            t = 0f;
            lastPosition = origin;
            transform.localPosition = origin;
        }

        radAngle = angle * Mathf.Deg2Rad;

        // Parametric curve : ballistic trajectory
        newPosition.x = Mathf.Cos(radAngle) * speed * t;
        newPosition.y = -1f / 2f * -Physics.gravity.y * t * t + Mathf.Sin(radAngle) * speed * t + height;

        t += velocity * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Vector3 direction = (origin + newPosition - lastPosition);
        transform.localPosition += direction;
        lastPosition = transform.localPosition;

        //transform.localPosition = origin + newPosition;
    }
}
