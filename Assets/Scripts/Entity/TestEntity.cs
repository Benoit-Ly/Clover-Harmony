using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TestEntity : MonoBehaviour {

	// Use this for initialization
	public virtual void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
	}

    public virtual void FixedUpdate()
    {

    }

    public virtual void Move(float x, float y)
    {

    }

    public virtual void Attack()
    {

    }

    public virtual void Jump()
    {

    }

    public virtual void GetReadyForFusion()
    {

    }

    public virtual void ExecuteFusion(Transform target)
    {

    }

    public virtual void Defuse(Vector3 direction)
    {

    }
}
