using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WipEnemy : MonoBehaviour {

    [SerializeField]
    int health;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (health <= 0)
            Destroy();
	}

    void Destroy()
    {
        Destroy(gameObject);
    }
}
