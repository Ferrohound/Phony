using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles save files in the main menu. 
/// Creates new games, loads current ones, and deletes user selected ones.
/// </summary>
public class TitleScreen : MonoBehaviour {

    List<Save> loadedSaves;

	void Start () {
        loadedSaves = new List<Save>();
        EnsureSaveDirectory();
        LoadSaveFiles();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void EnsureSaveDirectory() {
        if (!Directory.Exists("Saves")) {
            //Directory.CreateDirectory("Saves");
        }
    }
    private void LoadSaveFiles() {

        loadedSaves.Sort((x,y) => x.lastModified.CompareTo(y.lastModified));
    }
    public void LoadGame(int n) { 
        
    }
    public void NewGame(string name) { 
        
    }
    public void DeleteSave(int n) { 
        
    }
}