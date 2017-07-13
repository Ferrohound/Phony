using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//breaks out of arg1's follow coroutine

public class StopFollow : Command {
	
	public override void execute(string[] args)
	{
		Debug.Log("Stopping Dialogue jump");
		
		Character.Get(args[0]).GetComponentInChildren<NPCWander>().following = false;
	}
	
}

public partial class Cmd
{
	public void StopFollow(List<string> args)
	{
		Debug.Log("Stopping Dialogue jump");
	}
}