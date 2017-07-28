using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : ScriptableObject
{
	//public List ItemData;
	//use .Find() to get the thing
	public /*static */List<ItemContainer> items;
	//need a key of items instead
	public /*static */Dictionary<string, GameObject> ItemBank = new Dictionary<string, GameObject>();
	public Dictionary<string, Item> ExistingItemBank = new Dictionary<string, Item>();
	public bool initialized = false;
	
	public void setKey(string key, GameObject value)
	{
		ItemBank[key] = value;
	}
	
	public void setKey(string key, Item item)
	{
		ExistingItemBank[key] = item;
	}
	
	public void initialize()
	{
		//initialize the database upon game start
		for(int i = 0; i< items.Count; i++)
		{
			setKey(items[i].Name, items[i].gameObject);
			setKey(items[i].Name, items[i].item);
			items[i].item.ID = i;
			//Debug.Log(items[i].Name);
		}
	}
	
	void Awake()
	{
		
	}
}