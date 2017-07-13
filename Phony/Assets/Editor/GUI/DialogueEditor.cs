using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Dialogue))]
/*
	Custom editor for npc Dialogue
	probably unneeded
*/

public class DialogueEditor : Editor
{
	private Dialogue D;
	
	void Awake()
	{
		D = (Dialogue)target;
	}
	
	//overload the inspector display for tmpItem
	/*public override void OnInspectorGUI()
	{
		
		EditorUtility.SetDirty(D);
	}*/
	
}