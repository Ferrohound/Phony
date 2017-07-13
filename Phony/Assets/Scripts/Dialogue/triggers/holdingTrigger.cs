using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class holdingTrigger : MonoBehaviour {
	
	public string thing;
	public Dialogue speaker;
	public string tableVal;
	public string beginVal;
	public int targetNode;
	public int defaultNode;
	
	void OnTriggerStay(Collider col)
	{
		if(col.tag!="Player")
			return;
		
		if(Dialogue.tasks == null || !Dialogue.tasks.ContainsKey(beginVal))
		{
			return;
		}
		
		if(Dialogue.tasks.ContainsKey(tableVal))
			Destroy(this);
		
		//================================================ RE ADD THIS
		//set the dialogue node to the target node
		/*if(col.GetComponentsInChildren<PlayerInteraction>()[0].IsHolding(thing))
		{
			Debug.Log("STOP.");
			speaker.setReset(targetNode);
			Dialogue.tasks[beginVal] = false;
		}
		else if (!Dialogue.tasks.ContainsKey(tableVal))
		{
			//Dialogue.tasks[beginVal] = true;
			speaker.setReset(defaultNode);
		}
		*/
	}
}
