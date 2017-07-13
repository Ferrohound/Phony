using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

//create dropdown menu
public class ItemAsset : MonoBehaviour {

	[MenuItem("Assets/Create/ItemDatabase")]
	
	public static void CreateAsset()
	{
		ScriptableObjectUtility.CreateAsset<ItemDatabase>();
	}
}