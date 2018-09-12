using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

    [SerializeField]
    float backgroundSize;
    [SerializeField]
    bool scrolling;
    [SerializeField]
    bool paralax;
    [SerializeField]
    float parallaxSpeed;
    //[SerializeField]
    //float viewZone = 10;

    Transform cameraTransform;
    Transform[] layers;
    int leftIndex, rightIndex;
    float lastCameraX;

	// Use this for initialization
	void Start ()
    {
        cameraTransform = Camera.main.transform;
        lastCameraX = cameraTransform.position.x;
        layers = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; ++i)
        {
            layers[i] = transform.GetChild(i);
        }

        leftIndex = 0;
        rightIndex = layers.Length - 1;
	}

    private void Update()
    {
        if (paralax)
        {
            float deltaX = cameraTransform.position.x - lastCameraX;
            transform.position += Vector3.right * (deltaX * parallaxSpeed / 10);
        }

        lastCameraX = cameraTransform.position.x;

        if (scrolling)
        {
            if (cameraTransform.position.x < (layers[leftIndex].transform.position.x))
                ScrollLeft();

            if (cameraTransform.position.x > (layers[rightIndex].transform.position.x))
                ScrollRight();
        }
    }

    void ScrollLeft()
    {
        layers[rightIndex].position = Vector3.right * (layers[leftIndex].position.x - backgroundSize);

        leftIndex = rightIndex;
        --rightIndex;

        if (rightIndex < 0)
            rightIndex = layers.Length - 1;
    }

    void ScrollRight()
    {
        layers[leftIndex].position = Vector3.right * (layers[rightIndex].position.x + backgroundSize);

        rightIndex = leftIndex;
        ++leftIndex;

        if (leftIndex == layers.Length)
            leftIndex = 0;
    }
}
