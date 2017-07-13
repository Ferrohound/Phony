using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Physics : MonoBehaviour {
    Rigidbody r;
	void Start () {
        r = GetComponent<Rigidbody>();
        r.AddTorque(new Vector3(100000F, 100000F, 100000F), ForceMode.Impulse);
        Debug.Log("Stuff happened");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}