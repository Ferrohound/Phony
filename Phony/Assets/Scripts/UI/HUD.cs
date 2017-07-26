using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {
	public static GameObject RightPickUp, LeftPickUp, RightThrow, LeftThrow, Combine, Cursor;
	//for simplicity, just assign these + prefab so it's always the same
	public GameObject TRightPickUp, TLeftPickUp, TRightThrow, TLeftThrow, TCombine, TCursor;
	// Use this for initialization
	void Start () {
		//assign the HUD stuffs
		RightPickUp = TRightPickUp;
		LeftPickUp = TLeftPickUp;
		RightThrow = TRightThrow;
		LeftThrow = TLeftThrow;
		Combine = TCombine;
		Cursor = TCursor;
		
		if(GameObject.FindWithTag("Player") == null)
			Cursor.SetActive(false);
		else
			Cursor.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	//call this to turn these UI elements on and off
	void Animate(GameObject ui, bool activate)
	{
		
	}
}
