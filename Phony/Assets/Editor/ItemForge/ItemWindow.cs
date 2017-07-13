using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

//custom thing for editing items

[CustomPropertyDrawer(typeof(Item))]
public class ItemWindow : PropertyDrawer
{
	//public Item I;
	
	/*
	void Awake()
	{
		I = (Item)target;
	}
	*/
	
	//override that sexy gui
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//DrawDefaultInspector();
	}
	
}