﻿using UnityEngine;
using System.Collections;

public class BoltGrower : MonoBehaviour {

    public float growRate = 2f;

	void Start ()
    {
	
	}

	void Update ()
    {
        transform.localScale = new Vector3(transform.localScale.x + growRate * Time.deltaTime, transform.localScale.y + growRate * Time.deltaTime, transform.localScale.z + growRate * Time.deltaTime);
	}
}