using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;


	//inheritance method
public class Command : MonoBehaviour {
	public virtual void execute(string[] args) {}
}


//partial class method
public partial class Cmd
{
	
}