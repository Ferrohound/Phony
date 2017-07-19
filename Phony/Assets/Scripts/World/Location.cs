using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//so we can reference positions within the world for NPC movement
//and camera pans, etc..
//create an empty game object and attach this script to it

public class Location : MonoBehaviour {
	
	public static Dictionary<string, Transform> Locations = 
		new Dictionary<string, Transform>();
	public string Name;
	
	void Awake()
	{
		Locations[Name] = transform;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
