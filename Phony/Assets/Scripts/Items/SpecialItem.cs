using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[System.Serializable]

//special items get NPCs to react to them while being held
//they will also become negated by the npc's dialogue post-calls once their needed node
//is reached
public class SpecialItem : GameItem
{
	//static list holding all of the special items
	public static Dictionary<string, SpecialItem> SpecialItems = 
		new Dictionary<string, SpecialItem>();
		
	Dictionary<string, int> NPCTargets;
	Dictionary<string, int> NPCResets;
		
	//ideally you'd load this from a file, but we'll do it manually here
	public List<string> NPCs;
	public List<int> TargetNodes;
	//this one gets called and updated when the item is picked up
	public List<int> originalReset;
	
	void Awake()
	{
		//add yourself to the dictionary
		SpecialItems[item.name] = this;
		NPCTargets = new Dictionary<string, int>();
		for(int i= 0 ; i<TargetNodes.Count; i++)
		{
			NPCTargets[NPCs[i]] = TargetNodes[i];
		}
		NPCResets = new Dictionary<string, int>();
	}
	
	void changeReset(string npc, int reset)
	{
		NPCResets[npc] = reset;
	}
	
	//TO DO=====================================================================
	void changeNPCStart(string npc)
	{
		//if it's -1, it's been negated
		if(NPCTargets[npc] == -1)
			return;
		//call changeReset here
		//changeReset(npc, npc.resetNode)
		//change npc's reset here
		//npc.reset = NPCResets[npc];
	}
}