using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Saves all global information in a run. 
/// </summary>
[System.Serializable]
public class Save {
    #region MetaData
    public string saveName;
    public System.DateTime creationDate;
    public System.DateTime lastModified;
    #endregion

    #region SceneLocation
    //which door should be loaded for each scene. Default is 0.
    public string[] scenes;
    public int[] doorID;
    #endregion

    /// <summary>
    /// Saves the save file to a location in the file directory.
    /// </summary>
    /// <param name="path">Location in file directory</param>
    /// <param name="save">Save object</param>
    public static void SaveTo(string path, Save save) {
        try {
            Stream s = File.Open(path, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(s, save);
            s.Close();
        } catch(Exception e) {
            Debug.LogError(e.Message);
        }
    }
    /// <summary>
    /// Loads the savefile in a specified directory and returns the Save object
    /// Returns null if there is an error.
    /// </summary>
    /// <param name="path">Location in file directory</param>
    /// <returns></returns>
    public static Save LoadSave(string path) {
        try {
            Stream s = File.Open(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            Save loadedSave = (Save)formatter.Deserialize(s);
            s.Close();
            return loadedSave;
        } catch (Exception e) {
            Debug.LogError(e.Message);
            return null;
        }
    }
}