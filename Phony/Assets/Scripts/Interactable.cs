using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Allows the player to interact with the object.
/// </summary>
public interface Interactable {
    /// <summary>
    /// Passes the playercontroller script and a flag to the object being interacted with. 
    /// </summary>
    /// <param name="pc">PlayerController script</param>
    /// <param name="flag">A flag denoting information, to be decided upon by the script.</param>
    void Interact(PlayerController pc, int flag);
}

/// <summary>
/// Allows the object to be controlled by another object. Still in development
/// </summary>
public interface Controllable {

    void Drive(Interactable driver, int flag);

    void Check(Interactable driver, int flag);
}