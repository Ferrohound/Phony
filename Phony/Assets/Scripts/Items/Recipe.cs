using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

/*
	Recipe class
	
*/

[XmlRoot("Recipe")]
public class Recipe{
	
	[XmlElement("_name")]
	public string _name;
	
	[XmlElement("_item1")]
	public string _item1;
	
	[XmlElement("_dst1")]
	public bool _dst1;
	
	[XmlElement("_item2")]
	public string _item2;
	
	[XmlElement("_dst2")]
	public bool _dst2;
	
	
	public Recipe(){}
	
	public Recipe(string name, string item1, string item2)
	{
		_name = name;
		_item1 = item1;
		_item2 = item2;
	}
	
	public void setItem1(string name, bool dst)
	{
		_item1 = name;
		_dst1 = dst;
	}
	
	public void setItem2(string name, bool dst)
	{
		_item2 = name;
		_dst2 = dst;
	}
}

//ingredient class in case things don't work out with normal tuples
//use as key for hash table
//interchangeable so <A,B> == <B,A>, A and B being the names of the items
public class Ingredients: System.IEquatable<Ingredients>, IEqualityComparer<Ingredients>
{
	public Ingredients(int id, string item1, string item2)
	{
		ID = id;
		i1 = item1;
		i2 = item2;
	}
	
	public int ID { get; private set; }
	public string i1 { get; private set; }
	public string i2 { get; private set; }
	
	public bool Equals(Ingredients x, Ingredients y)
    {
		return x.Equals(y);
	}
	
	public bool Equals(Ingredients other)
	{
		Debug.Log(i1 + " " + i2);
		Debug.Log(other.i1 + " " + other.i2);
		
		return other.ID == ID || 
			(i1 == other.i1 && i2 == other.i2) || 
			(i1 == other.i2 && i2 == other.i1);
	}
	
	//to allow use in dictionary
	public override int GetHashCode()
	{
		//https://stackoverflow.com/questions/13019307/make-a-unique-hash-out-of-two-strings
		//Xor for unique result
		//if the two strings are equal, this might be a problem
		return i1.GetHashCode() ^ i2.GetHashCode();
		//return ID.GetHashCode();
	}
	
	//to allow use in dictionary
	public int GetHashCode(Ingredients i)
	{
		return i.GetHashCode();
	}
}
