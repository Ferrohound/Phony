﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/*
	Use this to manage the tasks that the player has achieved
    It also sets the player location to any door in the scene. 
*/

public class ProgressManager : MonoBehaviour {
	
	public static ProgressManager Instance;
	
	
	//dialogue's tasks
	public static Dictionary<string, bool> tasks;
	public static Dictionary<string, bool> chapterTasks;
	//each character's reset node
	public static Dictionary<string, int> resetNodes;
	
	public static List<string> taskList;
	
	public static int doorID = 0; //current saved door
    [HideInInspector]
    public static Dictionary<int, Door> Doors; //dictionary of all the doors in each scene by ID
    [HideInInspector]
    public static Dictionary<string, int> sceneLoc; //dictionary of saved door for scene

    public static PlayerController pc;
    private static Rigidbody pc_rb;
    private static CapsuleCollider pc_cc;
	
	void Awake(){
        if (Instance == null){
			DontDestroyOnLoad(gameObject);
			Instance = this;
            if (sceneLoc == null){ //will have to change so that this will be loaded from a save file
                sceneLoc = new Dictionary<string, int>();
                sceneLoc[UnityEngine.SceneManagement.SceneManager.GetActiveScene().name] = 0;
            }
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += new UnityAction<Scene, Scene>(UponSceneChange);
		} else if(Instance != this){
			Destroy(gameObject);
		}
        //=======================
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pc_rb = pc.gameObject.GetComponent<Rigidbody>();
        pc_cc = pc.gameObject.GetComponent<CapsuleCollider>();
    }

    private void UponSceneChange(Scene T0, Scene T1) {
        //Debug.Log(T0.name + "," + T1.name); //works
        Doors = new Dictionary<int, Door>();
        foreach (GameObject dr in GameObject.FindGameObjectsWithTag("Door")){
            try{
                Door d = dr.GetComponent<Door>();
                Doors[d.DoorID] = d;
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }
        string name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!sceneLoc.ContainsKey(name)) {
            sceneLoc[UnityEngine.SceneManagement.SceneManager.GetActiveScene().name] = 0;
        }
        setPlayerLocation(pc.transform);
        ActivatePlayerPhysics();
    }

    void Start(){
		
	}
	
	public static void setPlayerLocation(Transform player){
        /*
        //load scene doesn't exist in context
		if(LoadScene.doors.ContainsKey(doorID)){
			PlayerController pc = player.GetComponent<PlayerController>();
			pc.posOld = LoadScene.doors[doorID].position;
			pc.posNew = LoadScene.doors[doorID].position;
			player.transform.position = LoadScene.doors[doorID].position;
		}*/
        if (!Doors.ContainsKey(doorID)) {
            Debug.LogError("Door " + doorID + " does not exist in the scene.");
        } else {
            int i = sceneLoc[UnityEngine.SceneManagement.SceneManager.GetActiveScene().name];
            player.position = Doors[i].getPosition().position;
        }
        pc.ReloadAfterSave();
	}

    public static void SetSceneDoor(string scene, int door) {
        sceneLoc[scene] = door;
    }

    public static void DeactivatePlayerPhysics() {
        pc_rb.isKinematic = true;
        pc_cc.enabled = false;
    }
    public static void ActivatePlayerPhysics() {
        pc_rb.isKinematic = false;
        pc_rb.velocity = Vector3.zero;
        pc_cc.enabled = true;
    }
}
