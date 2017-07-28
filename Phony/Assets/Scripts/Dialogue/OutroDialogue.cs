using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutroDialogue : MonoBehaviour {
	
	public Dialogue dialogue;
	
	// Use this for initialization
	void Start () {
		if(dialogue!=null)
			dialogue.runDialogue();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
