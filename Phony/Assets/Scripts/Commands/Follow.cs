using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//argument 1: thing that will follow
//argument 2: thing 1 will follow

public class Follow : Command {
	
	public override void execute(string[] args)
	{
		Character.Get(args[0]).GetComponentInChildren<NPCWander>().following = true;
		
		Character.Get(
			args[0]).
			GetComponentInChildren<NPCWander>().startFollow(Character.Get(args[1]));		
	}
	
}

public partial class Cmd
{
	public void Follow(List<string> args)
	{
		Debug.Log("Following");
	}
}