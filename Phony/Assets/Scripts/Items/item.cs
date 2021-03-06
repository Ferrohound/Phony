﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class Item{
	
	public enum Attributes {slimy, wet, absorbent, metal, hard, glass, mushy, fire, flamable};
	
	//maybe give them an ID too
	//outer tag
	[XmlAttribute("_name")]
	public string name;

    //Whether the player requires two hands to hold an object
    [XmlAttribute("_bothHands")]
    public bool _bothHands;
	
	//item ID for item generation
	[XmlAttribute("_ID")]
	public int ID;
	
	//inner elements of the attribute
	[XmlElement("_attributes")]
	public List<Attributes> _attributes;
	public Attributes _attribute;
	
	[XmlElement("_special")]
	public bool special;
	
	//[XmlElement("Durability")]
	//public float durability;
	
    /// <summary>
    /// General constructor
    /// </summary>
	Item(){}
	
	public bool hasAttribute(Attributes attribute){
		return _attributes.Contains(attribute);
	}
	
	public bool hasAttribute(Attributes[] attributes){
		foreach(Attributes item in attributes){
			if(!hasAttribute(item))
				return false;
		}
		return true;
	}
}
