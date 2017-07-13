using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Saves metadata about a save file.
/// 
/// Also saves all necessary data about a run. 
/// </summary>
[System.Serializable]
public class Save {
    public string saveName;
    public System.DateTime creationDate;
    public System.DateTime lastModified;
}