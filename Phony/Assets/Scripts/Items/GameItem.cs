using System.Collections;
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
	
    public Item item;
	public string itemName;
	public string itemTest;
    public bool held; //used for trigger checking in 3d person
    private Rigidbody rb;
    private Collider col;
    void Start() {
		
		if(iDatabase == null)
			iDatabase = DB;
		
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        held = false;
		//load item from database via name
		LoadItem(itemName);
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
			item = iDatabase.ExistingItemBank[name];
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
	        Physics.IgnoreCollision(col, pc.m_Capsule, false);
            transform.SetParent(null);
            held = false;
            break;
		case 1:
            rb.isKinematic = true;
			Physics.IgnoreCollision (col, pc.m_Capsule, true);
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
			Physics.IgnoreCollision (col, pc.m_Capsule, true);			
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
		
		if (result == null)
			return;
		
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
}