using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFX : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LaunchFX(GameObject particle)
    {


        particle.GetComponent<ParticleSystem>().Play(true);
    }
}
