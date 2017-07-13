using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Point source of gravity, modeled as a sphere of constant mass of radius r. 
/// Outside of the radius, gravity is inversely proportional to the distance squared.
/// Inside the radius, gravity is proportional to the distance. 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Gravity : Abr_Gravity {
    private Vector3 temp;                           //use for internal calculations
    private float forceC;                          //use for gravity when r < radius. For force, multiply by object mass

    private Rigidbody m_Rigidbody;

    /// <summary>
    /// Gravitational parameter
    /// </summary>
    public float mu {
        get { return World.GravitationalConstant * m_Rigidbody.mass; }
    }
    public float radius;                            //radius at which inverse linear goes to inverse quadratic

    void FixedUpdate() {
        collt = Physics.OverlapSphere(center, 1000F);
        for (int i = 0; i < collt.Length; i++) { 
            r = collt[i].GetComponent<Rigidbody>();
            if(r != null){
                AddWorldForce(r);
            }
        }
    }

    public override Vector3 getForce(Rigidbody obj) {
        Vector3 v = center - obj.position;
        if (v.magnitude > radius) {
            v = World.GravitationalConstant * m_Rigidbody.mass * obj.mass / v.sqrMagnitude * v.normalized;
        } else {
            v = forceC * obj.mass * v.normalized * (v.magnitude / radius);
        }
        return v;
    }

    public override Vector3 getForce(Vector3 p, float m) {
        Vector3 v = center - p;
        if (v.magnitude > radius){
            v = World.GravitationalConstant * m_Rigidbody.mass * m / v.sqrMagnitude * v.normalized;
        } else {
            v = forceC * m * v.normalized * (v.magnitude / radius);
        }
        return v;
    }
    public override void InitializeVariables() {
        center = gameObject.transform.position;
        m_Rigidbody = GetComponent<Rigidbody>();
        if (radius == 0) radius = 150f;
        forceC = World.GravitationalConstant * m_Rigidbody.mass / Mathf.Pow(radius, 2);
    }
}