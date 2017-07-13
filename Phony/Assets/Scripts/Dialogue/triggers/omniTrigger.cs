﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//upon creation, change the dialogue node or scene
public class omniTrigger : MonoBehaviour {
	
	public string path;
	public int index;
	public Dialogue target;
	public bool newPart = false;
	public bool runImmediate = false;
	public bool setAuto = false;

	// Use this for initialization
	void Start () {
		if(newPart){
			target.loadDialogue(path);
			target.auto = setAuto;
			if(runImmediate)
				target.runDialogue();
			Destroy(this);
		}
		else{
			target.setNext(index);
			target.auto = setAuto;
			if(runImmediate){
				target.runDialogue();
			}
			Destroy(this);
		}
	}
}
