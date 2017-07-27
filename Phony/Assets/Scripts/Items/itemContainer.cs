using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

/*
	Serializable data containor for each individual item
*/
[System.Serializable]
public class ItemContainer{
	
	public string Name;
	//public enum Attributes {slimy, wet};
	public Item.Attributes Attribute;
	public Item item;
	
	//audio clips
	//public AudioClip[] audioClips; //getting object composition depth limit warnings
	//public ItemContainer[] Items;
	public Texture2D Icon;
	public GameObject gameObject;
}


//class to hold all of the items; the ItemDatabase
/*
	"The ScriptableObject class will give us the magic of being able to turn our 
	dialogue class into our own custom asset files."

	Holds a list of itemContainors
*/

