﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

/*
	NPC Dialogue class
	Handles all of the dialogue runnning.
	
	Needs to have a canvas in the scene (doesn't have to be attached to camera)
	player could have a hash table <string, bool> of tasks to perform
	for dialogue options and triggers, could check event name as such to see if 
	the player has done it
*/

public class Dialogue : MonoBehaviour {
	
	//=======================================================MEMBER VARIABLES
	//the dialogue tree that's being referenced
	DialogueTree dialogue;
	
	public new string name = "";
	public Animator animator;
	
	public bool coolingDown = false;
	bool skip = false;
	
	//path to the dialogue
	public string Path;
	
	//the dialogue box info
	GameObject Name;
	GameObject nodeText;
	public GameObject dialogueWindow;
	public GameObject buttonPrefab;
	RectTransform ParentPanel;
	
	//list of options from the current node, and list of options the player can actually
	//choose from
	GameObject[] options;
	GameObject[] availableOptions;

	//cameras to use
	public Camera mainCamera;
	public Camera dialogueCamera;

	//LayerMask for raycasting, so the player could ie) speak to them through a glass 
	//wall
	public LayerMask layer;
	public float talkDistance = 5f;
	public float talkCooldown = 2f;
	
	//Audio
	public AudioSource Source;
	public AudioClip Voice;
	public AudioClip Enter;
	public AudioClip Exit;
	//public static AudioSource InitiateSound;
	
	//coroutines
	private IEnumerator runCoroutine;
	private IEnumerator displayCoroutine;
	
	//node selection
	//by default, the selected node is -2; dialogue will loop until -2 isn't selected
	private int select = -2;
	int nodeID = -1;
	int numOptions = 0;
	Node current;
	
	//Auto would cause the dialogue to run once the player enters the
	//trigger area
	public bool auto = false;
	
	//static bool to keep track if a dialogue is running
	public static bool running = false;
	
	public static bool endEpisode = false;
	
	//whether or not the text is currently scrolling
	bool textScroll = false;
	
	//the list of tasks and achievements the player has accomplished or rather, hasn't
	public static Dictionary<string, bool> tasks;
	public static Dictionary<string, bool> chapterTasks;
	public static List<string> taskList;
	public static Dictionary<string, GameObject> Characters;
	
	//script for whitenoise script
	private Whitenoise whitenoise;
	
	
	// Use this for initialization
	void Start () {
		Character.characters[name] = gameObject;

		whitenoise = gameObject.GetComponent<Whitenoise>();
		if(whitenoise == null)
			Debug.Log("whoops, "+name+"'s whitenoise is null!");
		
		
		//since the main camera has a don't destroy on load
		if(mainCamera == null)
			mainCamera = Camera.main;
		
		if(dialogueCamera == null)
			dialogueCamera = mainCamera;
		
		//for focusing the camera on the appropriate characters
		if(name!="")
		{
			if(Characters == null)
				Characters = new Dictionary<string, GameObject>();
			Characters[name] = gameObject;
		}
		
		//load the dialogue from the given path
		dialogue = DialogueTree.Load(Path);
		
		//find the required components and initialize the private variables
		var canvas = GameObject.Find("Canvas");
		
		initiateTasks();
		
		Source = gameObject.GetComponent<AudioSource>();
		
		dialogueWindow = Instantiate<GameObject>(dialogueWindow);
		dialogueWindow.transform.SetParent(canvas.transform, false);
		
		RectTransform windowTrans = (RectTransform)dialogueWindow.transform;
		windowTrans.localPosition = new Vector3(0,-100,0);
		
		
		Name = dialogueWindow.transform.Find("SpeakerPanel").gameObject.transform.Find("[npc]").gameObject;
		
		options = new GameObject[4];
		availableOptions = new GameObject[4];
		
		ParentPanel = dialogueWindow.transform.Find("ButtonPanel").gameObject.GetComponent<RectTransform>();
		
		nodeText = dialogueWindow.transform.Find("TextPanel").gameObject.transform.Find("[dialogue]").gameObject;
		
		dialogueWindow.SetActive(false);
	}
	
	void Update()
	{
		if(coolingDown)
		{
			talkCooldown+=Time.deltaTime;
		}
		if(coolingDown && talkCooldown>0.5f)
		{
			talkCooldown = 0.5f;
			coolingDown = false;
		}
		
		//skip to the end of the current text
		skip = Input.GetButtonDown("Fire1");
	}
	
	//============================================================= DIALOGUE HELPER FUNCTIONS
	//change the reset node
	public void setReset(int reset)
	{
		dialogue._next = reset;
	}
	
	//load a new dialogue 
	public void loadDialogue(string newPath){
		Path = newPath;
		
		//load the dialogue from the given path
		dialogue = DialogueTree.Load(Path);
		select = -2;
		nodeID = -1;
	}
	
	//update the text
	private void updateText(Node node){
		
		//if there is no name, don't display the name box
		if(node._name == null || node._name == "")
		{
			if(Name.transform.parent!=null)
				Name.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			if(Name.transform.parent!=null)
			{
				Name.transform.parent.gameObject.SetActive(true);
				Name.GetComponent<Text>().text = node._name;
			}
		}
		
		//if text is still scrolling in the coroutine, stop it
		if(displayCoroutine!=null)
			StopCoroutine(displayCoroutine);
		
		//start text scrolling
		displayCoroutine = DisplayText(node._text);
		StartCoroutine(displayCoroutine);
		
		//get a list of the options, and available options
		options = new GameObject[node._options.Count];
		availableOptions = new GameObject[node._options.Count];
		
		//loop through all of this node's possible options and display them
		int j=0;
		for(int i=0;i<node._options.Count;i++){
			if(updateButton(node, node._options[i], i))
			{
				availableOptions[j] = options[i];
				j++;
			}
		}
		numOptions = j;
	}
	
	//update the butotn info
	bool updateButton(Node node, dialogueOption option, int index)
	{
		//check if there's a requirement for this option to be available
		if(option._req != null && option._req != ""){
			//if the dictionary[option._req] == false, return
			if(!tasks[option._req])
				return false;
		}
		
		//create a new button, set its parent and set its scale
		GameObject newButton = (GameObject) Instantiate(buttonPrefab);
		newButton.transform.SetParent(ParentPanel, false);
		newButton.transform.localScale = new Vector3(1, 1, 1);
		
		//set the button's text
		newButton.GetComponentInChildren<Text>().text = option._text;
		Button tmpButton = newButton.GetComponent<Button>();
		
		//add an event listener to the button, once it's selected, go to the
		//dialogue option's destination node
		tmpButton.onClick.AddListener(delegate{
		setSelect(option._dest);});
		
		options[index] = newButton;
		
		//display the options in displayOptions
		newButton.SetActive(false);
		return true;
	}
	
	//display the possible options after the text has stopped scrolling
	void displayOptions()
	{
		//if the number of options are greater than 1, or the option has text, display it
		if(numOptions>1 || availableOptions[0].GetComponentInChildren<Text>().text!="")
		{
			for(int i=0; i<numOptions; i++)
			{
				availableOptions[i].SetActive(true);
			}
			availableOptions[0].GetComponent<Button>().Select();
		}
	}
	
		//method for selecting the next dialogue node to load
	public void setSelect(int x){
		select = x;
	}
	
	public void setNext(int x){
		dialogue._next = x;
	}
	
	
	public void runDialogue(){
		if(GameObject.FindGameObjectWithTag("Player")!=null)
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
		
		if(mainCamera == null)
			mainCamera = Camera.main;
		
		if(dialogueCamera == null)
			dialogueCamera = Camera.main;
		
		mainCamera.gameObject.SetActive(false);
		dialogueCamera.gameObject.SetActive(true);
		
		running = true;
		auto = false;
		
		runCoroutine = run();
		StartCoroutine(runCoroutine);
	}
	
	
	
	//initiate the tasks, load them from progress manager
	private void initiateTasks()
	{
		if(ProgressManager.tasks == null)
		{
			ProgressManager.tasks = new Dictionary<string, bool>();
		}	
		
		if(ProgressManager.chapterTasks == null)
		{
			ProgressManager.chapterTasks = new Dictionary<string, bool>();
		}
		
		
		if(ProgressManager.taskList == null)
		{
			ProgressManager.taskList = new List<string>();
		}	
		
		//iterate through all of the tasks and add them in
		for(int i=0; i<dialogue._tasks.Count; i++)
		{
			ProgressManager.chapterTasks[dialogue._tasks[i]] = false;
			ProgressManager.tasks[dialogue._tasks[i]] = false;
			ProgressManager.taskList.Add(dialogue._tasks[i]);
		}
		
		
		tasks = ProgressManager.tasks;
		chapterTasks = ProgressManager.chapterTasks;
		taskList = ProgressManager.taskList;
		
		Dictionary<string, int> tmp = null;
		//get the correct reset node
		if(ProgressManager.resetNodes == tmp)
		{
			//Debug.Log("The resetNodes thing is null, re-initializing");
			ProgressManager.resetNodes = new Dictionary<string, int>();
		}
		
		if(!ProgressManager.resetNodes.ContainsKey(name))
		{
			//Debug.Log("No next value assigned for " + name);
			ProgressManager.resetNodes[name] = dialogue._next;
		}
		
		dialogue._next = ProgressManager.resetNodes[name];
		
		//Debug.Log(ProgressManager.resetNodes[name]);
	}

	public static void saveState()
	{
		Debug.Log("Saving State.");
		ProgressManager.tasks = tasks;
		ProgressManager.chapterTasks = chapterTasks;
		ProgressManager.taskList = taskList;
	}
	
	public static void loadState()
	{
		tasks = ProgressManager.tasks;
		chapterTasks = ProgressManager.chapterTasks;
		taskList = ProgressManager.taskList;
	}
	
	
	
	//add the achievement to the hash table
	public void achieve(string thing){
		//Debug.Log(thing + " HAS BEEN ACHIEVED");
		tasks[thing] = true;
	}
	
	//cycle through all tasks, if they're all complete, you win!
	//slash, move on to the next episode
	void checkStatus(){
	for(int i=0; i<chapterTasks.Count;i++){
			Debug.Log("CYCLING..." + taskList[i]);
			if(tasks[taskList[i]] == false)
				return;
		}
		Debug.Log("YOU DID IT!");
		endEpisode = true;
		//end the game
	}
	
	
	//=============================================================IENUMERATORS
	//run the dialogue tree coroutine
	public IEnumerator run()
	{
		
		if( whitenoise!=null)
			whitenoise.Play();
		
		if(Enter!=null)
			Source.PlayOneShot(Enter);
		
		//THIS WILL DO FOR NOW
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		
		if(animator!=null)
		{
			//animator.SetBool("InConversation", true);
			animator.Play("conversation");
		}
		
		dialogueWindow.SetActive(true);
		
		//disable the overhead UI
		IndicatorOverlay overlay = transform.GetComponent<IndicatorOverlay> ();
		if(overlay!=null)
		{
			overlay.enabled = false;
		}
		
		
		nodeID = dialogue._next;
		
		//while the node isn't an exit node...
		while(nodeID!=-1)
		{
			current = dialogue._nodes[nodeID];
			updateText(current);
			//testing out the execute function
			current._precalls.ForEach ((Call c) => c.execute ());

			//add accomplishment to dictionary
			if(current._accomplish!=null && 
				current._accomplish!="")
			{
				tasks[current._accomplish] = true;
				Debug.Log("ACCOMPLISHED " + current._accomplish);
				checkStatus();
			}
			
			//default value for select
			select = -2;
			
			int index = 0;
			int direction;
			float selCooldown = 0.25f;
			int j = numOptions;
			while(select == -2)
			{
				if(!textScroll)
				{
					//availableOptions[index].GetComponent<Button>().Select();
					//get the direction the player is moving the controller for selection
					direction = -(int) Input.GetAxisRaw("Vertical");
							
					if(selCooldown<0){
						index+=direction;
							
						//bind the indices
						if(index<0)
							index = 0;
						if(index>j-1)
							index = j-1;
					
					}

					//cooldown for changing the selection
					if(direction!=0 && selCooldown<0){
						selCooldown = 0.25f;
					}
					
					//decrement the cooldown
					selCooldown -= Time.deltaTime;

					//if the player presses fire1, the text isn't scrolling 
					//invoke that button's click event and move the dialogue along
					if (Input.GetButtonDown("Fire1") && !textScroll && running 
						&& selCooldown<0)
					{
						availableOptions[index].GetComponent<Button>().onClick.Invoke();
						selCooldown = 0.25f;
						
					}
				}
				yield return /*new WaitForEndOfFrame()*/null;
			}
			//destroy the buttons
			for(int i=0; i<current._options.Count;i++)
			{
				Destroy(options[i]);
			}
			
			//call the postcalls, change the reset node 
			current._postcalls.ForEach ((Call c) => c.execute ());
			dialogue._next = current._reset;
			nodeID = select;
		}
		
		//play sound for exiting dialogue
		if(Exit!=null)
			Source.PlayOneShot(Exit);
		
		//change camera's back, set everything back to its default
		running = false;
		coolingDown = true;
		talkCooldown = 0f;
		dialogueCamera.gameObject.SetActive(false);
		mainCamera.gameObject.SetActive(true);
		dialogueWindow.SetActive(false); 
		
		//store the reset node
		ProgressManager.resetNodes[name] = dialogue._next;
		//Debug.Log(name + "'s reset node should now be " + ProgressManager.resetNodes[name]);
		
		//if the episode is to be ended..
		if(endEpisode)
		{
			var canvas = GameObject.Find("Canvas");
			canvas.transform.Find("EndDay").gameObject.SetActive(true);
		}
		/*else
		{
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
		}*/
		if(GameObject.FindGameObjectWithTag("Player")!=null)
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
		
		//play the idle animation
		if(animator!=null)
			animator.Play("idle");
		
		//re-enable the overhead display
		if(overlay!=null)
		{
			overlay.enabled = true;
		}
	}
	
	//coroutine for displaying the text
	//handle text wrapping too
	private IEnumerator DisplayText(string displayText){
		int strLen = displayText.Length;
		int index = 0;
		
		nodeText.GetComponent<Text>().text = "";
		textScroll = true;
		
		//================================================================TEXT DELAYS ==========
		float dDelay = 0.01f;
		float tmpDelay = dDelay;
		float pDelay = 0.3f;
		
		bool dummy = false;
		
		int callCount = 0;
		
		while(true){
			
			dummy = Input.GetButtonUp("Fire1");
			
			//if the player presses Fire1, just put the text and get out
			if (skip){
				dummy = false;
				nodeText.GetComponent<Text>().text = (string)displayText;
				yield return new WaitForSeconds(dDelay);
				break;
			}
			
			//deal with newline character
			if(displayText[index] == '\\' && index<strLen-1 && displayText[index+1] == 'n'){
				index++;
				nodeText.GetComponent<Text>().text+='\n';
			}
			else if((displayText[index] == '!' || displayText[index] == '?' ||
				displayText[index] == '.') && index<strLen-1 && 
				(displayText[index+1] == ' '|| displayText[index+1] == '\n')){
					yield return new WaitForSeconds(pDelay);
				}
			
			//mid-text functions
			else if(displayText[index] == '%')
			{
				switch(displayText[index+1])
				{
					//execute a midcall
					case 'c':
						//if(callCount>current._midcalls.Count)
							//current._midcalls[callCount].execute();
						index++;
						callCount++;
					break;
					
					//pause the dialogue for a moment
					case 'p':
					index++;
					break;
					
					//toggle bold
					case 'b':
					index++;
					break;
					
					//increase textspeed
					case 'i':
					index++;
					tmpDelay += 0.2f;
					break;
					
					//decrease textspeed
					case 'd':
					index++;
					tmpDelay -= 0.2f;
					break;
					
					//reset textSpeed
					case 'r':
					index++;
					tmpDelay = dDelay;
					break;
					
					default:
					break;
				}
			}
			//otherwise go normally
			//probably move back to previous position
			else{
				nodeText.GetComponent<Text>().text += displayText[index];
				
				if(Voice!=null)
					Source.PlayOneShot(Voice);
			}
			
			index++;
			
			if(index<strLen){
				//play a sound potentially
				//wait for a moment before adding next character
				yield return new WaitForSeconds(tmpDelay);
			}
			else{
				break;
			}
		}
		displayOptions();
		textScroll = false;
	}
	
	//face the player
	IEnumerator Face(Transform t)
	{
		float curr = Time.deltaTime;
		float dur = 1.5f;
		yield return null;
	}
	
	
	
	//==================================================================TRIGGERS
	/*void OnTriggerEnter(Collider col){
		//Debug.Log(col.tag);
		if(col.tag == "Player" && auto && !running){
			Vector3 p4 = col.transform.TransformDirection(Vector3.forward);
			float PDotN = Vector3.Dot(p4, transform.position - col.transform.position);
			
			RaycastHit hit;
			//raycast from player, the player's forward, store it in hit, of distance hit
			if(Physics.SphereCast(col.transform.position, 1, p4, out hit, talkDistance, layer))
			{
				//if the ray hits this character, run the thing
				if(hit.collider.gameObject == this.transform.parent.gameObject)
				{
					if(mainCamera == null)
						mainCamera = Camera.main;
					
					if(transform.parent!= null && transform.parent.parent!=null)
						transform.parent.parent.LookAt(col.transform);
					
					col.transform.parent.LookAt(transform);
					
					mainCamera.gameObject.SetActive(false);
					dialogueCamera.gameObject.SetActive(true);
					running = true;
					//GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().enabled = false;
					runDialogue();
					auto = false;
				}
			}
		}
	}*/
	
	//have a talking cooldown so the player doesn't automatically jump back when they
	//exit conversation
	/*void OnTriggerStay(Collider col){
		if(auto || col.tag!="Player")
			return;
		
		//if the player presses "interact"/Fire1, disable player movement and
		//run the dialogue tree
		if (Input.GetButtonDown("Fire1") && !running && !coolingDown){
			Vector3 p4 = col.transform.TransformDirection(Vector3.forward);
			float PDotN = Vector3.Dot(p4, transform.position - col.transform.position);
			
			RaycastHit hit;
			//raycast from player, the player's forward, store it in hit, of distance hit
			if(Physics.SphereCast(col.transform.position, 1, p4, out hit, talkDistance, layer))
			{
				//if the ray hits this character, run the thing
				if(hit.collider.gameObject == this.transform.parent.gameObject)
				{
					if(mainCamera == null)
						mainCamera = Camera.main;
					
					if(transform.parent!= null && transform.parent.parent!=null)
						transform.parent.parent.LookAt(col.transform);
					
					col.transform.parent.LookAt(transform);
					
					mainCamera.gameObject.SetActive(false);
					dialogueCamera.gameObject.SetActive(true);
					running = true;
					//GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().enabled = false;
					runDialogue();
				}
			}
			//if the player is too close to raycast, it's probably alright
			else if(Vector3.Distance(col.transform.position, transform.position) < 2
				&& PDotN>0.75)
			{
				if(mainCamera == null)
					mainCamera = Camera.main;
				
				if(transform.parent!= null && transform.parent.parent!=null)
						transform.parent.parent.LookAt(col.transform);
				
				mainCamera.gameObject.SetActive(false);
				dialogueCamera.gameObject.SetActive(true);
				running = true;
				//GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().enabled = false;
				runDialogue();
			}
		}
			
	}*/
	
}
