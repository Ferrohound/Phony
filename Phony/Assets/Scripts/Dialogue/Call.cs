/*
	Call class;
	Executes a method, _m with _args & _intArgs as parameters
	via reflection
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

using System;
using System.Reflection;


[XmlRoot("Call")]
public class Call
{
	
	public enum Script
	{
		CameraEffect,
		PlayAnimation,
		StopFollow,
		Follow
	};
	
	/*
		list of arguments
		list of lists
		add block=true

		<Call>
			<target></target>
			<script>ScriptName</script>
			<arg>A</arg>
			<arg>B</arg>
			<arg>C</arg>
		</Call>
	*/
	
	[XmlAttribute]
	public string _m;
	[XmlElement("_target")]
	public string _target;
	[XmlElement("_args")]
	public List<string> _args;
	[XmlElement("_intArgs")]
	public List<int> _intArgs;
	
	//[XmlElement("_arg")]
	//public List<string> _arguments;
	
	//[XmlArray("_lst")]
	//[XmlArrayItem("Lst")]
	//public List<Lst> _lst;
	
	Call() {}
	
	//execute the function - Patrick's old code
	/*
	public void execute(){
		var obs = new List<object>();
		for(int i=0;i<_arguments.Count;i++){
			obs.Add(_arguments[i]);	
		}
		
		for(int i=0;i<_lst.Count;i++){
			obs.Add(_lst[i]._s.ToArray());	
		}
		
		GameEvents.Call(_m, obs.ToArray());
	}*/
	
	public void execute()
	{
		//check if script is valid ==============================
		
		//=======================================================
		
		//constant time
		//GameObject tmp = Character.Get(_target);
		
		//get the type of the script =============================================
		Type t = Type.GetType(_m);
		ConstructorInfo tCon = t.GetConstructor(Type.EmptyTypes);
		//create a command object ================================================
		object command = tCon.Invoke(new object[]{});
		
		//Debug.Log("Executing Script Now");
		//reflection
			
		//get the execute method of this function ===================================
		MethodInfo execMethod = t.GetMethod("execute");
		if(execMethod!=null)
		{
			//convert arguments to object
			var obs = new List<object>();
			obs.Add(_args.ToArray());
			//obs.Add(_intArgs.ToArray());
			//for(int i=0;i<_args.Count;i++){
				//obs.Add(_args[i]);	
			//}
			
			//execute the method with the list of objects, throw error if fails =========
			try
			{
			execMethod.Invoke (command, obs.ToArray());
			}catch (Exception ex){
				if (ex is TargetParameterCountException) 
				{
					Debug.LogErrorFormat ("Method {0} improper argument count. Had {1} needed {2}", "execute", _args.Count, execMethod.GetParameters().Length);
				}
				else if(ex is ArgumentException)
				{
					Debug.LogErrorFormat ("Method {0} could not convert parameters. Check format.", "execute");
				}
				Debug.LogException (ex);
			}
			//Debug.Log("Wowza");
		}
	}
	
};

[XmlRoot("Lst")]
public class Lst{

		[XmlElement("_s")]
		public List<string> _s;
		
		Lst(){}
	
	
}
