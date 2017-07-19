using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraEffect : Command {
	
	public enum CamEffects
	{
		FocusOn,
		ZoomIn,
		ZoomOut,
		Pan,
		ColourChange
	};
	
	
	public override void execute(string[] args)
	{
		//figure out what camera effect to do, then do it
		Debug.Log("Using Camera Effect");
	}
	
	
}

public partial class Cmd
{
	public void CameraEffect(List<string> args)
	{
		Debug.Log("Stopping Dialogue jump");
	}
}