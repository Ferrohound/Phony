using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Character class; in charge of npc interactions, states/activity
	Works as one big state machine
	
	NPC would inherit from this class and implement their own work, etc.. functions

*/

public class Character : MonoBehaviour{
	
	public static Dictionary<string, GameObject> characters = 
		new Dictionary<string, GameObject>();

    private World world;
		
	Dialogue dialogue;
	//0 default, 1 story, 2 banter?
	Dialogue[] dialogues;
	
	//queue of actions
	Queue<IEnumerator> actions = new Queue<IEnumerator>();
	Schedule schedule;
	
	IEnumerator current = null;
	IEnumerator tmp = null;
	
    /// <summary>
    /// States the npc can be in
    /// Story state makes them immune to state changes induced by the calendar
    /// </summary>
	public enum npcState{work, play, story, sleep};
	public npcState State = npcState.play;
	
	void Start(){
		schedule = new Schedule();
		//load schedule here
		//==================================================
		//finished loading schedule
        world = GameObject.FindGameObjectWithTag("CONTROL").GetComponent<World>(); //Get world script to have access for game time.
		
		dialogue = gameObject.GetComponent<Dialogue>();
	}
	
	void Update(){
		if(State == npcState.play)
			Play();
		else if(State == npcState.work)
			Work();
		else
			Sleep();
	}
	
	void Work(){
		
	}
	
	void Play(){
		
	}
	
	void Sleep(){
		
	}
	
	//run dialogue
	void Talk(int flag, Collider player)
	{
		switch (flag)
		{
			//default talk
			case 0:
				transform.LookAt(player.transform);
				dialogue.runDialogue();
			break;
			
			//giving item talk
			case 1:
			break;
		}
	}
	
	
	public static GameObject Get(string name){
		Debug.Log(name);
		
		if(name == "Player")
			return GameObject.Find("Player");
		
		if(characters.ContainsKey(name))
		{
			Debug.Log("Success!");
			return characters[name];
		}
		else
		{
			Debug.Log("Failed.");
			return null;
		}
	}
	
	//==========================================================IEnumerator stuff
	
	public void AddAction(IEnumerator action)
	{
		actions.Enqueue(action);
	}
	
	//coroutine queue here
	private IEnumerator execute()
	{
		while(true)
		{
			if(actions.Count>0)
			{
				current = actions.Dequeue();
				yield return StartCoroutine(current);
			}
			else
			{
				yield return null;
			}
		}
	}
	
	public void StopLoop()
	{
		if(current!=null)
		{
			StopCoroutine(current);
		}
	}
	
	//change to boolso can use HUD
	void RunDCheck(Collider col)
	{
		//if the player presses "interact"/Fire1, disable player movement and
		//run the dialogue tree
		Vector3 p4 = col.transform.TransformDirection(Vector3.forward);
		float PDotN = Vector3.Dot(p4, transform.position - col.transform.position);
		
		RaycastHit hit;
		//raycast from player, the player's forward, store it in hit, of distance hit
		if(Physics.SphereCast(col.transform.position, 1, p4, out hit, 
			dialogue.talkDistance, dialogue.layer))
		{
			//if the ray hits this character, run the thing
			if(hit.collider.gameObject == this.transform.parent.gameObject)
			{
				Talk(0, col);
			}
		}
		else if(Vector3.Distance(col.transform.position, transform.position) < 2
			&& PDotN>0.75)
		{
			Talk(0, col);
		}
	}
	
	//==================================================================TRIGGERS
	void OnTriggerStay(Collider col){
		if(dialogue == null)
		{
			Debug.Log("Dialogue isn't initialized!");
			return;
		}
		
		if(dialogue.auto || col.tag!="Player")
			return;
		
		if (Input.GetButtonDown("Fire1") && !Dialogue.running && !dialogue.coolingDown){
			RunDCheck(col);
		}
	}
	
	//have a talking cooldown so the player doesn't automatically jump back when they
	//exit conversation
	void OnTriggerEnter(Collider col)
	{
		if(col.tag != "Player")
			return;
		
		if(dialogue == null)
		{
			Debug.Log("Dialogue isn't initialized!");
			return;
		}
		if(!dialogue.auto)
			return;
		
		//if the tag isn't player, if the dialogue isn't auto, or if a dialogue is already running
		//return home
		dialogue.auto = false;
		Talk(0, col);
	}
}