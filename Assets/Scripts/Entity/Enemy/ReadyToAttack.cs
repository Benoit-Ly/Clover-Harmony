using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyToAttack : MonoBehaviour {

    [SerializeField]
    float changeValue = 0.0002f;

    float maxValue;
    bool changeDirection = false;

    void Start()
    {
        maxValue = transform.position.x - 0.5f;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x - changeValue * Time.deltaTime, transform.position.y, transform.position.z);

        if (transform.position.x <= maxValue && !changeDirection)
        {
            maxValue = transform.position.x + 0.5f;
            changeValue *= -1;
            changeDirection = true;
        }

        else if (transform.position.x >= maxValue && changeDirection)
        {
            maxValue = transform.position.x - 0.5f;
            changeValue *= -1;
            changeDirection = false;
            Wait();
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
    }
}
