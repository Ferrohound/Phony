using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, Interactable {
    public Transform m_controlled; //Until I get a better idea of what to implement
    private Controllable m_driven; //actual thing needed to be attached to

    private Animator m_am;
    private AudioSource m_audio;
	// Use this for initialization
	void Start () {
        m_am = GetComponent<Animator>();
        m_audio = GetComponent<AudioSource>();
        m_driven = m_controlled.GetComponent<Controllable>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Interact(PlayerController pc, int flag) {
        if (m_audio != null) m_audio.Play();
        if(m_driven != null) m_driven.Drive(this, flag);
    }
}