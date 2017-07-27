using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : ScriptableObject
{
	//public List ItemData;
	//use .Find() to get the thing
	public /*static */List<ItemContainer> items;
	//need a key of items instead
	public /*static */Dictionary<string, GameObject> ItemBank;
	public Dictionary<string, Item> ExistingItemBank;
	
	public void setKey(string key, GameObject value)
	{
		ItemBank[key] = value;
	}
	
	public void setKey(string key, Item item)
	{
		ExistingItemBank[key] = item;
	}
	
	void Awake()
	{
		//initialize the database upon game start
		for(int i = 0; i< items.Count; i++)
		{
			setKey(items[i].Name, items[i].gameObject);
			setKey(items[i].Name, items[i].item);
		}
	}
}