﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
/*
	Contains the dialogue, the character speaking, 
	a list of dialogueOption objects for choices
	as well as a list of pre, post and mid calls; functions to be executed
	before, after and during dialogue respectively. 
	
	See ../Commands/Command.cs for additional details

*/
[XmlRoot("Node")]
public class Node{
	
	[XmlElement("_ID")]
	public int _ID;
	
	[XmlElement("_text")]
	public string _text;
	
	[XmlElement("_name")]
	public string _name;
	
	[XmlElement("_reset")]
	public int _reset;
	
	[XmlArray("_options")]
	[XmlArrayItem("dialogueOption")]
	public List<dialogueOption> _options;
	
	[XmlArray("_precalls")]
	[XmlArrayItem("Call")]
	public List<Call> _precalls;
	
	[XmlArray("_postcalls")]
	[XmlArrayItem("Call")]
	public List<Call> _postcalls;
	
	[XmlArray("_midcalls")]
	[XmlArrayItem("Call")]
	public List<Call> _midcalls;
	
	[XmlElement("_accomplish")]
	public string _accomplish;
	
	//for serialization
	public Node() {
		_options = new List<dialogueOption>();
	}
	
	public Node(string text){
		_text = text;
		_options = new List<dialogueOption>();
	}
	
	public void setID(int ID)
	{
		_ID = ID;
	}
	
	public void setText(string text)
	{
		_text = text;
	}
	
	public void setReset(int reset)
	{
		_reset = reset;
	}
	
	public void setName(string name)
	{
		_name = name;
	}
	
	public void setAccomplish(string accomplish)
	{
		_accomplish = accomplish;
	}
	
	public void addDialogueOption(dialogueOption option)
	{
		_options.Add(option);
	}
	
	public void removeDialogueOption(dialogueOption option)
	{
		_options.Remove(option);
	}
	
	public void addPrecall(Call call)
	{
		_precalls.Add(call);
	}
	
	public void removePrecall(Call call)
	{
		_precalls.Remove(call);
	}
	
	public void addMidcall(Call call)
	{
		_midcalls.Add(call);
	}
	
	public void removeMidcall(Call call)
	{
		_midcalls.Remove(call);
	}
	
	public void addPostcall(Call call)
	{
		_postcalls.Add(call);
	}
	
	public void removePostcall(Call call)
	{
		_postcalls.Remove(call);
	}
	
}
