﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[System.Serializable]
public class GameItem : MonoBehaviour, Interactable {
	
		
	//static item database
	//need to initialize when the game starts, ideally on the start screen
	public ItemDatabase DB;
	//static item database
	//need to initialize when the game starts, ideally on the start screen
	public static ItemDatabase iDatabase;
	
	Dictionary<Item.Attributes, GameObject> GOHash;
	Dictionary<Item.Attributes, bool> DeathHash;
	
	public Item.Attributes[] Affectors;
	public List<GameObject> Effects;
	public bool[] KillMe;
	
    public Item item;
	public string itemName;
	public string itemTest;
    public bool held; //used for trigger checking in 3d person
    private Rigidbody rb;
    private List<Collider> cols;
	bool initialized = false;
	
    void Start() {
		if(!initialized)
			Initialize();
    }
	
	public void Initialize()
	{
		//hash tables of what attributes affect this object, what to spawn and 
		//whether or not to self-destruct afterwards
		GOHash = new Dictionary<Item.Attributes, GameObject>();
		DeathHash = new Dictionary<Item.Attributes, bool>();
		
		for(int i=0; i<Effects.Count; i++)
		{
			GOHash[Affectors[i]] = Effects[i];
			DeathHash[Affectors[i]] = KillMe[i];
		}
		
		if(iDatabase == null)
			iDatabase = DB;
		
        rb = GetComponent<Rigidbody>();
        cols = new List<Collider>(GetComponents<Collider>());
        held = false;
		//load item from database via name
		LoadItem(itemName);
		initialized = true;
	}
	
    void FixedUpdate() { 
        
    }
    /// <summary>
    /// Use this to load an item into the GameItem container
    /// </summary>
    /// <param name="it">Item to be loaded</param>
    public void LoadItem(Item it) {
        item = it;
    }
	
	public void LoadItem(string name)
	{
		//error check
		if(iDatabase.ExistingItemBank.ContainsKey(name))
		{
			Debug.Log("Found "+name+"!");
			item = iDatabase.ExistingItemBank[name];
		}
	}
    /// <summary>
    /// 0 - drop item.
    /// 1 - pickup in left
    /// 2 - pickup in right
    /// </summary>
    /// <param name="pc"></param>
    public void Interact(PlayerController pc, int flag) {
        //prepare for extendability
        switch (flag) { 
		case 0:
            rb.isKinematic = false;
			for(int i = 0; i<cols.Count; i++)
			{
				Physics.IgnoreCollision(cols[i], pc.m_Capsule, false);
			}
            transform.SetParent(null);
            held = false;
            break;
		case 1:
            rb.isKinematic = true;
			for(int i=0; i<cols.Count;i++)
			{
				Physics.IgnoreCollision (cols[i], pc.m_Capsule, true);
			}
			//1st person mode
			if(pc.Perspective == 0){
				transform.SetParent(pc.m_inter.FPL);
			} else {
				transform.SetParent(pc.m_inter.leftT);
			}
            transform.localPosition = Vector3.zero;
            held = true;
            break;
        case 2:
            rb.isKinematic = true;
			for(int i=0; i<cols.Count;i++)
			{
				Physics.IgnoreCollision (cols[i], pc.m_Capsule, true);		
			}			
			//1st person
			if(pc.Perspective == 0){
				transform.SetParent(pc.m_inter.FPR);
			} else {
				transform.SetParent(pc.m_inter.rightT);
			}
            transform.localPosition = Vector3.zero;
            held = true;
            break;
        default:
                break;
        }
    }
    /// <summary>
    /// Use this to reenable collisions.
    /// </summary>
    /// <param name="pc">Collider to reenable collisions.</param>
    /// <returns></returns>
 /*   private IEnumerator reenableCollisions(Collider pc) {
        yield return new WaitForSeconds(0.3f);
        Physics.IgnoreCollision(col, pc, false);
     //   yield return null;
    }
*/

	void OnTriggerEnter(Collider col){
		GameItem other = col.transform.GetComponent<GameItem>();
		//check if the other object is a gameItem
		if(other == null)
			return;
		if(other.item == null)
			return;
		
		Ingredients ing = new Ingredients( 0, item.name, other.item.name);
		Recipe result = Recipes.Cook(ing);
		//no recipe for this exists, do it via attribute
		if (result == null)
		{
			AttributeCollision(other);
			return;
		}
		
		Debug.Log(result._name);
		//do other stuff============================================================TO DO
		//check if the itemBank contains that item
		if(!iDatabase.ItemBank.ContainsKey(result._name))
			return;
		
		GameObject newItem = iDatabase.ItemBank[result._name];
		//instantiate the item 
		Instantiate(newItem, this.transform.position, this.transform.rotation);
		
		
		//and destroy items accordingly===============================================TO DO
	}
	
	//check the item's attribute, spawn corresponding things, delete, etc..
	void AttributeCollision(GameItem other)
	{
		if(!GOHash.ContainsKey(other.item._attribute))
			return;
		
		Instantiate(GOHash[other.item._attribute], transform.position, transform.rotation);
		if(DeathHash[other.item._attribute])
			Destroy(this.gameObject);
	}
}