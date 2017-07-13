using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All types of gravity are to be sub classes of this class. 
/// This is so that the World script can access all sources of gravity. 
/// </summary>
public abstract class Abr_Gravity : MonoBehaviour {
    /// <summary>
    /// Array of colliders that are in region of influence. Calculation.
    /// </summary>
    protected Collider[] collt;

    /// <summary>
    /// For calculations involving gravity. Calculation.
    /// </summary>
    protected Rigidbody r;

    /// <summary>
    /// Center of gravity
    /// </summary>
    protected Vector3 center;
    
    void Start(){
        GameObject.FindGameObjectWithTag("CONTROL").GetComponent<World>().AllGravitySources.Add(this);
        InitializeVariables();
    }

    public abstract void InitializeVariables();
    public abstract Vector3 getForce(Rigidbody obj);
    public abstract Vector3 getForce(Vector3 p, float m);
    /// <summary>
    /// Adds the force to the object
    /// </summary>
    /// <param name="obj">Rigidbody that will be affected.</param>
    public void AddWorldForce(Rigidbody obj) {
        obj.AddForce(getForce(obj));
    }
}