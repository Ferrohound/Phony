using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//arg1 play's animation arg2

public class PlayAnimation : Command {
	
	public override void execute(string[] args)
	{
		GameObject npc = Character.Get(args[0]);
		
		if(npc == null)
			return;
		
		npc.GetComponentInChildren<Animator>().Play(args[1]);
	}
	
}

public partial class Cmd
{
	public void PlayAnimation(List<string> args)
	{
		Debug.Log("Playing Animation");
	}
}