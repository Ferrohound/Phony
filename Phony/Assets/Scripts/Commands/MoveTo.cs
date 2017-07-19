using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : Command {

	public override void execute(string[] args)
	{
		//Character.Get(args[0]).GetComponentInChildren<NPCWander>().following = true;
		
		Character.Get(
			args[0]).
			GetComponentInChildren<NPCWander>().startMoveTo(Location.Locations[args[1]]);		
	}
	
}

public partial class Cmd
{
	public void MoveTo(List<string> args)
	{
		Debug.Log("Moving To " + args[1]);
	}
}