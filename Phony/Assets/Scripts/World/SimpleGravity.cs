using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gravity in one direction. 
/// </summary>
public class SimpleGravity : Abr_Gravity {
    /// <summary>
    /// Direction and strength of gravity
    /// </summary>
    public Vector3 gravityVector;

    /// <summary>
    /// How big is the sphere that is used for gravity calculations
    /// </summary>
    public float radius;

    void FixedUpdate() {
        collt = Physics.OverlapSphere(center, radius);
        for (int i = 0; i < collt.Length; i++) {
            r = collt[i].GetComponent<Rigidbody>();
            if (r != null) {
                AddWorldForce(r);
            }
        }
    }

    public override Vector3 getForce(Rigidbody obj){
        return gravityVector * obj.mass;
    }
    public override Vector3 getForce(Vector3 p, float m){
        return gravityVector * m;
    }
    public override void InitializeVariables(){
        center = gameObject.transform.position;
    }
}