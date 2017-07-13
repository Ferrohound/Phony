/*
	Class that manages the dialogue; holds the nodes that make up the dialogue tree
	Also contains the tasks that the player needs to perform in order to finish the 
	chapter.
	Will likely move the task list to a seperate file
	
	Methods:
		addNode -> Adds node to the tree
		Load -> loads and returns a dialogue object
		Save -> Saves the current dialogue tree
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

//get the root tag with the information you want to deserialize
[XmlRoot("Dialogue")]
public class DialogueTree{
	
	//list of Node objects under _nodes
	[XmlArray("_nodes")]
	[XmlArrayItem("Node")]
	public List<Node> _nodes;
	
	[XmlElement("_tasks")]
	public List<string> _tasks;
	//where to start off the conversation the next time this
	//npc is spoken to
	public int _next = 0;
	
	public DialogueTree(){
		_nodes = new List<Node>();
	}
	
	public void addNode(Node node)
	{
		_nodes.Add(node);
	}
	
	//static dialogue loader for use of all instances of Dialogue
	public static DialogueTree Load(string path){
		
		TextAsset _xml = Resources.Load<TextAsset>(path);
		XmlDocument xmldoc = new XmlDocument();
		xmldoc.LoadXml(_xml.text);
		
		//_xml = (TextAsset) xmldoc;
		
		XmlSerializer serial = new XmlSerializer(typeof(DialogueTree));
		StringReader reader = new StringReader(_xml.text);
		
		DialogueTree dialogue = (DialogueTree) serial.Deserialize(reader);
		//Debug.Log(dialogue._nodes.Count);
		
		reader.Close();
		return dialogue;
	}
	
	//serialize the dialogue 
	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(DialogueTree));
 		using(var stream = new FileStream(path, FileMode.Create))
 		{
 			serializer.Serialize(stream, this);
 		}
	}
}
