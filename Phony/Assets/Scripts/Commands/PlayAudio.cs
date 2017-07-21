using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : Command {

	public override void execute(string[] args)
	{
		//audioClips being a dictionary of possible audio clips
		/*foreach(AudioClip clip in audioClips){
			//audio being the audio source
			audio.PlayOneShot(clip.name);
		}*/
	}
	
}

public partial class Cmd
{
	/*public void MoveTo(List<string> args)
	{
		Debug.Log("Playing " + args[1]);
	}*/
}