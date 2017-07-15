﻿using System;
using UnityEngine;

/// <summary>
/// Door class
/// It can be activated by pressing: q or e in First person, mouse button 1 in third.
/// Additionally, if the door is used to change scenes
/// walking into it should activate the door
/// </summary>
public class Door : MonoBehaviour , Interactable {
    public bool SceneChange = false; //whether opening this door actually changes the scene
    public string sceneToLoad = ""; //name of scene to load if necessary
    public int DoorID;

	public GameObject canvas;
    public GameObject Animated; //the gameobject that has the animation applied

    private Animator m_am;
    private AudioSource m_audio;
    private int state;

	void Start () {
        m_am = GetComponent<Animator>();
        m_audio = GetComponent<AudioSource>();
        state = 0; //closed
	}
	
    public void Interact(PlayerController pc, int flag) {
        if (SceneChange) {//if this door loads a new scene
            try {
                Open();
                AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneToLoad, UnityEngine.SceneManagement.LoadSceneMode.Single);
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
            return; 
        }
        if (state == 0) {
            Open();
            state = 1;
        } else {
            Close();
            state = 0;
        }
    }
    public Transform getPosition() {
        return gameObject.transform.GetChild(0).transform;
    }
    #region Animation Control
    private void Open() {

    }

    private void Close() {

    }
    #endregion
}