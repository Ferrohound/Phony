using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ItemDatabase))]

/*
	Custom editor for items
*/

/*
//script to initialize the ItemBank array 
//in this case, I is the itemDatabase
for(int i = 0; i<I.items.Count; i++)
{
	I.setKey(I.items[i].Name, I.items[i].gameObject);
}
*/

public class ItemEditor : Editor
{
	
	/*
	// Show this float in the Inspector as a slider between 0 and 10
    [Range(0.0F, 10.0F)]
    public float myFloat = 0.0F;
	*/
	private ItemDatabase I;
	public Texture2D tex;
	
	void Awake()
	{
		I = (ItemDatabase)target;
	}
	
	//overload the inspector display for tmpItem
	public override void OnInspectorGUI()
	{
		for(int i=0; i<I/*temDatabase*/.items.Count; i++)
		{
			//GUILayout.Label ("Custom Item Editor for " + I/*temDatabase*/.items[i].Name);
			DrawItemData(I/*temDatabase*/.items[i]);
		}
		EditorUtility.SetDirty(I);
		DrawDefaultInspector ();
	}
	
	public void DrawItemData(ItemContainer ic)
	{
		serializedObject.Update();
		
		//draw the texture
        /*
		Texture2D tex = (Texture2D) EditorGUILayout.ObjectField(
			"Icon", tex, typeof (Texture2D), false
			);
		*/
		if(tex)
		{
			GUILayout.Label(tex);
		}
		
		
		/*
		
		if(tex)
		{
			GUILayout.Label(tex);
		}*/
		
		//apparently can't do find....
		/*
		SerializedProperty Name = ic.FindProperty ("Name");
		SerializedProperty Attribute = ic.FindProperty ("Attribute");
		SerializedProperty Audio = ic.FindProperty ("audioClips");
		SerializedProperty ICs = ic.FindProperty ("items");
		SerializedProperty icon = ic.FindProperty("icon");
		*/
		
		EditorGUI.BeginChangeCheck();
		
		//EditorGUILayout.PropertyField(ic.Name, true);
		//EditorGUILayout.PropertyField(ic.Attribute, true);
		//EditorGUILayout.PropertyField(ic.Audio, true);
		//EditorGUILayout.PropertyField(ic.Icon, true);
		
		if(EditorGUI.EndChangeCheck())
             serializedObject.ApplyModifiedProperties();
		
	}
	
}