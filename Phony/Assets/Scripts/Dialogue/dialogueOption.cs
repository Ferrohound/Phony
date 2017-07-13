/*
	Dialogue option class
	Contains the text to be displayed for this option, and the destination node
	should this option be chosen.
	Also contains requirements for this option to be chosen
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[XmlRoot("dialogueOption")]
public class dialogueOption{
	
	//display text for dialogue option
	[XmlElement("_text")]
	public string _text;
	
	//the index of the destination node
	[XmlElement("_dest")]
	public int _dest;
	
	//the requirement for this node to be displayed
	[XmlElement("_req")]
	public string _req;
	
	dialogueOption() {}
	
	public dialogueOption(string text="", int dest=0){
		_text = text;
		_dest = dest;
	}
	
	public void setDst(int destination)
	{
		_dest = destination;
	}
	
	public void setReq(string req)
	{
		_req = req;
	}
	
	public void setText(string text)
	{
		_text = text;
	}
}
