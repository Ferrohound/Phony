using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : Command {

	public override void execute(string[] args)
	{
		Debug.Log("Loading " + args[0] + ", sending player to door "+args[1]);
		int doorID = int.Parse(args[1]);	
		string scene = args[0];
		
		Dialogue.saveState();
		ProgressManager.doorID = doorID;
		SceneTransition.setScene(scene);
		//change this!
		SceneManager.LoadScene(scene);
		//SceneTransition t = gameObject.AddComponent<SceneTransition>();
		//t.play = true;
	}
	
}

public partial class Cmd
{
	/*public void MoveTo(List<string> args)
	{
		Debug.Log("Loading " + args[1] + " Scene");
	}*/
}