using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour /*, Interactable*/ {

	//entry door script opens to
	public string entry;

	public GameObject canvas;

    private Animator m_am;
    private AudioSource m_audio;
    private int state;


	private SceneTransition newscene;
	// Use this for initialization


	void Start () {
        m_am = GetComponent<Animator>();
        m_audio = GetComponent<AudioSource>();
        state = 0; //closed

		//var newscene = canvas.GetComponent<SceneTransition> ();
	}
	
    void Interact(PlayerController pc, int flag) {
        if (state == 0) { 
            state = 1;
        } else {
            state = 0;
        }
    }

	void OnTriggerStay(Collider obj) {
		//opens to new scene
		if (obj.tag == "Player") {
			SceneTransition.setScene(entry);
		}
	}

}