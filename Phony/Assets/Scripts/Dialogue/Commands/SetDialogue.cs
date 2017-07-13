using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//arguments as follows : npc, reset, list<achievements>, npc, reset, list<achievements>

public class SetDialogue : Command {
	public override void execute(string[] args) 
	{
		Debug.Log("Setting Dialogue");
		/*Vector3 tmp = Camera.main.transform.position;
		tmp.y+=2;
		tmp.z+=2;
		Camera.main.transform.position = tmp;*/
	}
}

public partial class Cmd
{
	public void SetDialogue(List<string> args)
	{
		Debug.Log("Stopping Dialogue jump");
	}
}